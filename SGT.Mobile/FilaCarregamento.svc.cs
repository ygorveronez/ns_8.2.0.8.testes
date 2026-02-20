using System;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;

namespace SGT.Mobile
{
    public class FilaCarregamento : WebServiceBase, IFilaCarregamento
    {
        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento> ObterMotivosRetiradaFilaCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            var repositorioMotivo = new Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento(unitOfWork);
            var motivosRetiradaFilaCarregamento = repositorioMotivo.ConsultarMobile();
            var motivosRetiradaFilaCarregamentoRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento>();

            foreach (var motivo in motivosRetiradaFilaCarregamento)
            {
                motivosRetiradaFilaCarregamentoRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento()
                {
                    Codigo = motivo.Codigo,
                    Descricao = motivo.Descricao
                });
            }

            return motivosRetiradaFilaCarregamentoRetornar;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento> ObterMotivosAtendimento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivo = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamado> motivosAtendimento = repositorioMotivo.ConsultarMobile();
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento> motivosAtendimentoRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento>();

            foreach (Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivo in motivosAtendimento)
            {
                motivosAtendimentoRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento()
                {
                    Codigo = motivo.Codigo,
                    Descricao = motivo.Descricao,
                    Tipo = motivo.TipoMotivoAtendimento,
                    ExigirQrCode = motivo.ExigeQRCodeAbertura,
                    ExigirFoto = motivo.ExigeFotoAbertura
                });
            }

            return motivosAtendimentoRetornar;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento> ObterAtendimentosCarga(int codigoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioAtendimento = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoAnalise repositorioAnaliseAtendimento = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repositorioChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaAtendimentos = repositorioAtendimento.BuscarListaPorCargasDoMotorista(codigoMotorista, new List<TipoMotivoAtendimento>() { TipoMotivoAtendimento.Reentrega, TipoMotivoAtendimento.Retencao, TipoMotivoAtendimento.RetencaoOrigem });

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento> listaAtendimentosRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>();

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado atendimento in listaAtendimentos)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencias = repositorioChamadoOcorrencia.BuscarOcorrenciasPorChamado(atendimento.Codigo);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> analisesAtendimento = repositorioAnaliseAtendimento.BuscarPorChamado(atendimento.Codigo);

                listaAtendimentosRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento()
                {
                    Codigo = atendimento.Codigo,
                    Numero = atendimento.Numero,
                    Descricao = atendimento.Descricao,
                    ProtocoloCarga = atendimento.Carga.Codigo,
                    Tipo = atendimento.MotivoChamado.TipoMotivoAtendimento,
                    DescricaoCarga = atendimento.Carga.CodigoCargaEmbarcador,
                    CodigoMotivo = atendimento.MotivoChamado?.Codigo ?? 0,
                    DescricaoMotivo = atendimento.MotivoChamado?.Descricao ?? string.Empty,
                    TipoCliente = atendimento.TipoCliente,
                    CNPJCliente = atendimento.Destinatario?.CPF_CNPJ ?? 0,
                    DescricaoCliente = atendimento.Destinatario?.Descricao ?? string.Empty,
                    Situacao = atendimento.Situacao,
                    DescricaoSituacao = atendimento.Situacao == SituacaoChamado.Finalizado ? "Aprovado" : atendimento.DescricaoSituacao,
                    DataCriacao = atendimento.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                    DataRetencaoInicio = atendimento.DataRetencaoInicio.HasValue ? atendimento.DataRetencaoInicio.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataRetencaoFim = atendimento.DataRetencaoFim.HasValue ? atendimento.DataRetencaoFim.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataReentrega = atendimento.DataReentrega.HasValue ? atendimento.DataReentrega.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataEntradaRaio = atendimento.CargaEntrega?.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataSaidaRaio = atendimento.CargaEntrega?.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    TempoRetencao = atendimento.TempoRetencao,
                    RetencaoBau = atendimento.RetencaoBau,
                    Observacao = atendimento.Observacao,
                    PlacaReboque = atendimento.PlacaReboque,
                    Filial = atendimento.Carga.Filial?.Descricao ?? string.Empty,
                    Analises = analisesAtendimento == null ? string.Empty : string.Join(" / ", (from obj in analisesAtendimento select obj.Observacao)),
                    NumeroOcorrencia = string.Join(", ", listaOcorrencias.Select(o => o.NumeroOcorrencia)),
                    Ocorrencias = (
                        from obj in listaOcorrencias
                        select new Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Ocorrencia()
                        {
                            Codigo = obj.Codigo,
                            NumeroOcorrencia = obj.NumeroOcorrencia,
                            TipoOcorrencia = obj.TipoOcorrencia.Descricao,
                            DescricaoSituacaoOcorrencia = obj.DescricaoSituacao,
                            OrigemOcorrencia = obj.Carga?.Filial?.Descricao ?? string.Empty,
                            DestinoOcorrencia = string.Empty,
                            ValorOcorrencia = obj.ValorOcorrencia,
                            ObservacaoOcorrencia = obj.Observacao,
                            ParametroHoras = this.BuscarHorasPeriodo(obj.Codigo, unitOfWork)
                        }
                    ).ToList()
                });
            }

            return listaAtendimentosRetornar;
        }

        private decimal BuscarHorasPeriodo(int codigoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros ocorrenciaParametroPeriodo = repCargaOcorrenciaParametros.BuscarPorCargaOcorrenciaETipo(codigoOcorrencia, TipoParametroOcorrencia.Periodo);

            decimal horasPeriodo = 0;
            if (ocorrenciaParametroPeriodo != null)
            {
                if (ocorrenciaParametroPeriodo.TotalHoras == 0)
                {
                    TimeSpan diferenca = ocorrenciaParametroPeriodo.DataFim.Value - ocorrenciaParametroPeriodo.DataInicio.Value;
                    horasPeriodo = Convert.ToDecimal(diferenca.TotalHours);
                }
                else
                    horasPeriodo = ocorrenciaParametroPeriodo.TotalHoras;
            }
            return horasPeriodo;
        }

        private string SalvarImagem(Stream imagem, out string tokenImagem)
        {
            tokenImagem = "";
            string retorno = "";

            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = imagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;

            string extensao = ".jpg";
            if (extensao.Equals(".jpg") || extensao.Equals(".jpeg"))
            {
                string token = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                tokenImagem = token;
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tokenImagem);

                using (System.Drawing.Image t = System.Drawing.Image.FromStream(ms))
                {
                    Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                }
            }
            else
            {
                retorno = "A extensão do arquivo é inválida.";
            }
            return retorno;
        }

        #endregion

        #region Métodos Públicos

        public Retorno<bool> AdicionarPorPlaca(string token, int usuario, int empresaMultisoftware, string placa, int codigoTipoRetornoCarga)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorPlaca(unitOfWork, placa) ?? throw new WebServiceException("Veículo não encontrado.");
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));

                    servicoFilaCarregamento.Adicionar(veiculo, codigoTipoRetornoCarga, TipoServicoMultisoftware);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (BaseException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a chegada do veículo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> AdicionarPorPlacasAtreladas(string token, int usuario, int empresaMultisoftware, string placaTracao, string placaReboque, int codigoTipoRetornoCarga)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Veiculo tracao = ObterVeiculoPorPlaca(unitOfWork, placaTracao) ?? throw new WebServiceException("Tração não encontrada");
                    Dominio.Entidades.Veiculo reboque = ObterVeiculoPorPlaca(unitOfWork, placaReboque) ?? throw new WebServiceException("Reboqe não encontrado");
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));

                    servicoFilaCarregamento.AdicionarPorVeiculosAtrelados(tracao, reboque, codigoTipoRetornoCarga, TipoServicoMultisoftware);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (BaseException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a chegada do veículo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> AceitarCarga(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamentoMobile = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));
                    Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento filaCarregamento = servicoFilaCarregamentoMobile.AceitarCarga(motorista.Codigo, TipoServicoMultisoftware);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoSucesso(filaCarregamento);
                }
                catch (ServicoException excecao)
                {
                    var retorno = Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);

                    return retorno;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoExcecao("Ocorreu uma falha ao aceitar a carga");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> ConfirmarChegadaVeiculoPorCodigo(string token, int usuario, int empresaMultisoftware, int codigoFilaCarregamento)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));

                    servicoFilaCarregamento.ConfirmarChegadaVeiculo(codigoFilaCarregamento);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a chegada do veículo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<TipoRetornoConfirmarChegadaVeiculo> ConfirmarChegadaVeiculoPorPlaca(string token, int usuario, int empresaMultisoftware, string placa)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorPlaca(unitOfWork, placa) ?? throw new WebServiceException("Veículo não encontrado.");
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));

                    if (servicoFilaCarregamento.ConfirmarChegadaVeiculo(veiculo))
                        return Retorno<TipoRetornoConfirmarChegadaVeiculo>.CriarRetornoSucesso(TipoRetornoConfirmarChegadaVeiculo.Sucesso);

                    Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo.Criar(veiculo);

                    if (conjuntoVeiculo.IsPermiteEntrarFila())
                        return Retorno<TipoRetornoConfirmarChegadaVeiculo>.CriarRetornoSucesso(TipoRetornoConfirmarChegadaVeiculo.InformarTipoRetornoCarga);

                    return Retorno<TipoRetornoConfirmarChegadaVeiculo>.CriarRetornoSucesso(TipoRetornoConfirmarChegadaVeiculo.InformarReboque);
                }
            }
            catch (BaseException excecao)
            {
                return Retorno<TipoRetornoConfirmarChegadaVeiculo>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<TipoRetornoConfirmarChegadaVeiculo>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a chegada do veículo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao> DesatrelarReboque(string token, int usuario, int empresaMultisoftware, string QRCodeLocal)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorMotorista(unitOfWork, motorista.Codigo);
                    Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local = ObterLocal(unitOfWork, QRCodeLocal);

                    if (local == null)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao> locais = ObterLocais(unitOfWork, QRCodeLocal);

                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoSucesso(
                            new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao() { Finalizada = false, Locais = locais }
                        );
                    }

                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamentoMobile = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);

                    servicoFilaCarregamentoMobile.DesatrelarVeiculo(veiculo, local);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoSucesso(
                        new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao() { Finalizada = true, Locais = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao>() }
                    );
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoExcecao("Ocorreu uma falha ao desatrelar o veículo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> EntrarFilaCarregamento(string token, int usuario, int empresaMultisoftware, TipoFilaCarregamento tipoFilaCarregamento, string latitude, string longitude, int lojaProximidade)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    TipoFilaCarregamentoVeiculo tipoFilaCarregamentoVeiculo = tipoFilaCarregamento == TipoFilaCarregamento.Reversa ? TipoFilaCarregamentoVeiculo.Reversa : TipoFilaCarregamentoVeiculo.Vazio;
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));
                    Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento filaCarregamento = servicoFilaCarregamento.Adicionar(motorista.Codigo, tipoFilaCarregamentoVeiculo, latitude, longitude, lojaProximidade.ToString().ToBool(), TipoServicoMultisoftware);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoSucesso(filaCarregamento);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoExcecao("Ocorreu uma falha ao entrar na fila de carregamento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> Entrar(string token, int usuario, int empresaMultisoftware, int tipoRetornoCarga, string latitude, string longitude, int lojaProximidade)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));
                    Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento filaCarregamento = servicoFilaCarregamento.Adicionar(motorista.Codigo, tipoRetornoCarga, latitude, longitude, lojaProximidade.ToString().ToBool(), TipoServicoMultisoftware);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoSucesso(filaCarregamento);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoExcecao("Ocorreu uma falha ao entrar na fila de carregamento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> InformarDoca(string token, int usuario, int empresaMultisoftware, string hash)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);

                    servicoFilaCarregamento.InformarDoca(motorista.Codigo, hash);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                catch (ServicoException excecao)
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar a doca");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> RecusarCarga(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));

                    servicoFilaCarregamento.RecusarCarga(motorista.Codigo);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                catch (ServicoException excecao)
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao recusar a carga");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> SairFilaCarregamento(string token, int usuario, int empresaMultisoftware, int motivoRetiradaFilaCarregamento)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));

                    servicoFilaCarregamento.Sair(motorista.Codigo, motivoRetiradaFilaCarregamento);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                catch (ServicoException excecao)
                {
                    return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao sair da fila de carregamento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> SairReversa(string token, int usuario, int empresaMultisoftware, string hash)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));
                    Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento filaCarregamento = servicoFilaCarregamento.RemoverReversa(motorista.Codigo, hash);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoSucesso(filaCarregamento);
                }
                catch (ServicoException excecao)
                {
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoExcecao("Ocorreu uma falha ao sair da fila de carregamento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<int> SolicitarAtendimento(string token, int usuario, int empresaMultisoftware, string latitude, string longitude, int codigoMotivo, string cnpjCliente, bool retencaoBau, string placaReboque, string dataReentrega, int codigoCarga)
        {
            Servicos.Log.TratarErro("token " + token);
            Servicos.Log.TratarErro("usuario " + usuario);
            Servicos.Log.TratarErro("usuempresaMultisoftwareario " + empresaMultisoftware);
            Servicos.Log.TratarErro("cnpjCliente " + cnpjCliente);
            Servicos.Log.TratarErro("codigoCarga " + codigoCarga);
            Servicos.Log.TratarErro("latitude " + latitude);
            Servicos.Log.TratarErro("longitude " + longitude);
            Servicos.Log.TratarErro("placaReboque " + placaReboque);
            Servicos.Log.TratarErro("dataReentrega " + dataReentrega);

            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                if (usuario <= 0)
                    throw new WebServiceException("Usuário/Motorista não foi informado.");

                if (string.IsNullOrWhiteSpace(cnpjCliente))
                    throw new WebServiceException("CNPJ cliente não informado.");

                if (codigoCarga <= 0)
                    throw new WebServiceException("Codigo carga não foi informado.");

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile repConfiguracaoMobile = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMobile(unitOfWork);

                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    if (carga == null)
                        throw new WebServiceException("Carga não foi encontrada.");

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorClienteECarga(carga.Codigo, cnpjCliente.ToDouble());
                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cnpjCliente.ToDouble());

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargaPedidoPorCargaEClienteDestinatario(carga.Codigo, cnpjCliente.ToDouble());
                    Dominio.Entidades.Cliente cliente = cargaPedidos.Count > 0 ? cargaPedidos.FirstOrDefault().Pedido.Remetente : destinatario;

                    Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repMotivoChamado.BuscarPorCodigo(codigoMotivo);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMobile configuracaoMobile = repConfiguracaoMobile.BuscarConfiguracaoPadrao();

                    if (motivoChamado == null)
                        throw new WebServiceException("Motivo não foi encontrado.");

                    if (destinatario == null)
                        throw new WebServiceException("Cliente não foi encontrado.");

                    if (cargaEntrega == null)
                        throw new WebServiceException("Nenhuma entrega encontrada.");

                    if (!cargaEntrega.DataEntradaRaio.HasValue)
                        throw new WebServiceException("Data de chegada não foi informada.");

                    if (codigoMotivo <= 0)
                        throw new WebServiceException("Motivo do Atendimento não foi informado.");

                    if (retencaoBau && !Utilidades.Validate.ValidarPlaca(placaReboque))
                        throw new WebServiceException("Placa informada é inválida.");

                    if (!retencaoBau && (motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao || motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem))
                    {
                        DateTime dataChegada = cargaEntrega.DataEntradaRaio.Value;
                        DateTime dataAtual = DateTime.Now;
                        DateTime horaInicioJanela = DateTime.MinValue;
                        DateTime horaFimJanela = DateTime.MinValue;
                        DateTime horaInicioCarga = dataChegada;

                        Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorOrigemEDestino(cliente?.CPF_CNPJ ?? 0, destinatario?.CPF_CNPJ ?? 0);
                        if (clienteDescarga != null)
                        {
                            horaInicioJanela = new DateTime(dataChegada.Year, dataChegada.Month, dataChegada.Day, int.Parse(clienteDescarga.HoraInicioDescarga.Substring(0, 2)), int.Parse(clienteDescarga.HoraInicioDescarga.Substring(3, 2)), 0);
                            horaInicioCarga = horaInicioJanela;
                            horaFimJanela = new DateTime(horaInicioJanela.Year, horaInicioJanela.Month, horaInicioJanela.Day, int.Parse(clienteDescarga.HoraLimiteDescarga.Substring(0, 2)), int.Parse(clienteDescarga.HoraLimiteDescarga.Substring(3, 2)), 0);

                            if (horaInicioJanela > horaFimJanela && horaInicioJanela.Date == horaFimJanela.Date)
                                horaFimJanela = horaFimJanela.AddDays(1);

                            bool cargaDomingo = false;

                            if (!clienteDescarga.Domingo && horaInicioCarga.DayOfWeek == DayOfWeek.Sunday)
                            {
                                horaInicioCarga = horaInicioCarga.AddDays(1);
                                cargaDomingo = true;
                            }

                            if (horaInicioJanela > horaFimJanela)
                            {
                                if (horaInicioJanela.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    horaInicioJanela = horaInicioJanela.AddDays(1);
                                    horaFimJanela = horaInicioJanela.AddDays(1);
                                    horaInicioCarga = horaInicioJanela;
                                }
                                else
                                {
                                    horaInicioJanela = horaInicioJanela.AddDays(-1);
                                    horaInicioCarga = horaInicioJanela;
                                }
                            }

                            if (dataChegada > horaInicioCarga && dataChegada >= horaFimJanela)
                                horaInicioCarga = horaInicioCarga.AddDays(1);
                            else if (dataChegada > horaInicioCarga && dataChegada < horaFimJanela)
                                horaInicioCarga = dataChegada;

                            if (!clienteDescarga.Domingo && horaInicioCarga.DayOfWeek == DayOfWeek.Sunday)
                            {
                                horaInicioCarga = horaInicioCarga.AddDays(1);
                                cargaDomingo = true;
                            }
                            else
                                cargaDomingo = false;
                        }

                        if (motivoChamado.TipoOcorrencia?.HorasSemFranquia > 0)
                        {
                            if (dataChegada >= horaInicioJanela)
                            {
                                if (dataChegada.AddHours(motivoChamado.TipoOcorrencia.HorasSemFranquia) > dataAtual)
                                    throw new WebServiceException("Não é permitido solicitar atendimento antes de " + motivoChamado.TipoOcorrencia?.HorasSemFranquia + " horas de espera.");
                            }
                            else if (horaInicioCarga.AddHours(motivoChamado.TipoOcorrencia.HorasSemFranquia) > dataAtual)
                                throw new WebServiceException("Não é permitido solicitar atendimento antes de " + motivoChamado.TipoOcorrencia.HorasSemFranquia + " horas de espera conforme janela destino.");
                        }

                    }

                    if (carga.SituacaoCarga.IsSituacaoCargaNaoFaturada())
                        throw new WebServiceException("Não é permitido solicitar atendimento enquanto a carga não tiver documentos emitidos.");

                    if (configuracaoMobile.ValidarRaioCliente && !Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(cliente, latitude.Replace(".", ",").ToDouble(), longitude.Replace(".", ",").ToDouble()))
                        throw new WebServiceException("Você não está dentro do raio permitido.");

                    DateTime _dataReentrega = dataReentrega.ToDateTime();

                    Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado();

                    objChamado.Empresa = carga.Empresa;
                    objChamado.MotivoChamado = motivoChamado;
                    objChamado.Cliente = cliente;
                    objChamado.Destinatario = destinatario;
                    objChamado.Carga = carga;
                    objChamado.CargaEntrega = cargaEntrega;
                    objChamado.TipoCliente = Dominio.Enumeradores.TipoTomador.Destinatario;
                    objChamado.RetencaoBau = retencaoBau;
                    objChamado.PlacaReboque = placaReboque;

                    if ((motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao || motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem) && !retencaoBau)
                        objChamado.DataRetencaoInicio = cargaEntrega.DataEntradaRaio;

                    if (motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega && _dataReentrega != DateTime.MinValue)
                        objChamado.DataReentrega = _dataReentrega;

                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objChamado, motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, unitOfWork);

                    unitOfWork.CommitChanges();

                    if (chamado != null)
                    {
                        new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);
                        return Retorno<int>.CriarRetornoSucesso(chamado.Codigo);
                    }
                    else
                        return Retorno<int>.CriarRetornoExcecao("Atendimento não gerado");
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao solicitar atendimento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> AtualizarAtendimento(string token, int usuario, int empresaMultisoftware, int codigoAtendimento, string latitude, string longitude, string dataReentrega)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoAtendimento, true);

                    if (codigoAtendimento <= 0)
                        throw new WebServiceException("O atendimento não foi selecionado.");

                    if (chamado.MotivoChamado.TipoMotivoAtendimento != TipoMotivoAtendimento.Reentrega)
                        throw new WebServiceException("Não é permitido atualizar esse atendimento.");

                    if (chamado.Situacao != SituacaoChamado.Aberto)
                        throw new WebServiceException("O atendimento não está mais em aberto");

                    DateTime _dataReentrega = dataReentrega.ToDateTime();

                    if (_dataReentrega == DateTime.MinValue)
                        throw new WebServiceException("Data de reentrega não foi informada.");

                    if (_dataReentrega >= DateTime.Now.AddMinutes(5))
                        throw new WebServiceException("Data e hora de reentrega não pode ser maior que a atual.");

                    if (!Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(chamado.Destinatario, latitude.Replace(".", ",").ToDouble(), longitude.Replace(".", ",").ToDouble()))
                        throw new WebServiceException("Você não está dentro do raio permitido.");

                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                    {
                        Usuario = motorista,
                        OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario
                    };

                    chamado.DataReentrega = _dataReentrega;
                    repChamado.Atualizar(chamado, auditado);

                    unitOfWork.CommitChanges();

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar o atendimento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> CancelarAtendimento(string token, int usuario, int empresaMultisoftware, int codigoAtendimento)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                    Servicos.Embarcador.Chamado.Chamado srvChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                    if (codigoAtendimento <= 0)
                        throw new WebServiceException("O atendimento não foi selecionado.");

                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoAtendimento);

                    if (chamado.Situacao != SituacaoChamado.Aberto)
                        throw new WebServiceException("O atendimento não está mais em aberto");

                    if (chamado.CargaEntrega?.DataSaidaRaio.HasValue ?? false)
                        throw new WebServiceException("A saída já foi informada.");

                    unitOfWork.Start();

                    Dominio.Entidades.Usuario motorista = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                    srvChamado.CancelarChamado(chamado, unitOfWork, null, motorista, TipoServicoMultisoftware);

                    unitOfWork.CommitChanges();

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (BaseException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao cancelar o atendimento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<string> EnviarByteImagemAtendimento(Stream imagem)
        {
            Retorno<string> retorno = new Retorno<string>();
            retorno.Status = true;
            try
            {
                string tokenImagem = "";
                retorno.Mensagem = this.SalvarImagem(imagem, out tokenImagem);
                if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Status = false;
                    retorno.Objeto = "";
                }
                else
                {
                    retorno.Objeto = tokenImagem;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a imagem (stream)";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> EnviarImagemAtendimento(int usuario, int codigoAtendimento, int empresaMultisoftware, string tokenImagem, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        string caminhoTemp = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                        string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, tokenImagem);

                        try
                        {

                            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                            Repositorio.Embarcador.Chamados.Chamado repAtendimento = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                            if (motorista != null)
                            {
                                Dominio.Entidades.Embarcador.Chamados.Chamado atendimento = repAtendimento.BuscarPorCodigo(codigoAtendimento);

                                if (atendimento != null)
                                {
                                    if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                    {
                                        serOcorrencia.EnviarImagemAtendimento(atendimento.Codigo, tokenImagem, usuario, 0, unitOfWork);
                                    }
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "A imagem não foi enviada para o servidor.";
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O codigo informado não pertence a uma ocorrência válida";
                                }
                            }
                            else
                            {
                                retorno.Status = false;
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                            }

                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            throw;
                        }
                        finally
                        {

                            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

                            unitOfWork.Dispose();
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da imagem";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> InformarDataChegada(string token, int usuario, int empresaMultisoftware, string dataChegada, string cnpjCliente, string latitude, string longitude, int codigoCarga)
        {
            Servicos.Log.TratarErro("InformarDataChegada cnpjCliente " + cnpjCliente);
            Servicos.Log.TratarErro("InformarDataChegada codigoCarga " + codigoCarga);

            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                if (codigoCarga <= 0)
                    throw new WebServiceException("A carga não foi selecionada.");

                if (string.IsNullOrWhiteSpace(cnpjCliente))
                    throw new WebServiceException("CNPJ cliente não informado.");

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
                    if (carga == null)
                        throw new WebServiceException("Carga não foi encontrada.");

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorClienteECarga(carga.Codigo, cnpjCliente.ToDouble());
                    if (cargaEntrega == null)
                        throw new WebServiceException("Nenhuma entrega encontrada.");

                    if (cargaEntrega.DataEntradaRaio.HasValue)
                        throw new WebServiceException("Data de chegada já foi informada");

                    DateTime.TryParseExact(dataChegada, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data);

                    try
                    {
                        unitOfWork.Start();

                        carga.DataAtualizacaoCarga = DateTime.Now;
                        cargaEntrega.DataEntradaRaio = data;
                        cargaEntrega.DataLimitePermanenciaRaio = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.CalcularPermanenciaMaxima(cargaEntrega, unitOfWork);

                        repositorioCarga.Atualizar(carga);
                        repositorioCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar a chegada");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> InformarDataSaida(string token, int usuario, int empresaMultisoftware, string dataSaida, string cnpjCliente, string senhaCliente, string latitude, string longitude, int codigoCarga)
        {
            Servicos.Log.TratarErro("InformarDataSaida cnpjCliente " + cnpjCliente);
            Servicos.Log.TratarErro("InformarDataSaida codigoCarga " + codigoCarga);
            Servicos.Log.TratarErro("InformarDataSaida senhaCliente " + senhaCliente);

            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                if (codigoCarga <= 0)
                    throw new WebServiceException("A carga não foi selecionada.");

                if (string.IsNullOrWhiteSpace(cnpjCliente))
                    throw new WebServiceException("CNPJ cliente não informado.");

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                    if (carga == null)
                        throw new WebServiceException("Carga não foi encontrada.");

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorClienteECarga(carga.Codigo, cnpjCliente.ToDouble());
                    if (cargaEntrega == null)
                        throw new WebServiceException("Nenhuma entrega encontrada.");

                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjCliente.ToDouble());
                    if (cliente == null)
                        throw new WebServiceException("Cliente não foi encontrado.");

                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                    if (chamado == null)
                        throw new WebServiceException("Atendimento não foi encontrado.");

                    if (cargaEntrega.DataSaidaRaio.HasValue)
                        throw new WebServiceException("Data de saída já foi informada.");

                    if (string.IsNullOrWhiteSpace(cliente.SenhaLiberacaoMobile) || !cliente.SenhaLiberacaoMobile.Equals(senhaCliente))
                        throw new WebServiceException("Senha informada é inválida.");

                    DateTime.TryParseExact(dataSaida, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data);
                    if (data > DateTime.Now.AddMinutes(5))
                        throw new WebServiceException("Data e hora de saída não pode ser maior que a atual.");

                    try
                    {
                        unitOfWork.Start();

                        cargaEntrega.DataSaidaRaio = data;
                        chamado.DataRetencaoFim = data;

                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                        repChamado.Atualizar(chamado);

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar a saída");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion

        #region Métodos Públicos de Consulta

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento> ObterDadosFilaCarregamento(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento filaCarregamento = servicoFilaCarregamento.ObterDadosFilaCarregamento(motorista.Codigo, TipoServicoMultisoftware);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoSucesso(filaCarregamento);
                }
                catch (ServicoException excecao)
                {
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamento>.CriarRetornoExcecao("Ocorreu uma falha ao obter dados da fila de carregamento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<string> ObterDetalhesCarga(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);
                    string detalhesCarga = servicoFilaCarregamento.ObterDetalhesCarga(motorista.Codigo);

                    return Retorno<string>.CriarRetornoSucesso(detalhesCarga);
                }
                catch (ServicoException excecao)
                {
                    unitOfWork.Rollback();

                    return Retorno<string>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();

                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha ao obter os detalhes da carga");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> ObterDadosCarga(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga carga = servicoFilaCarregamento.ObterDadosCarga(motorista.Codigo);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>.CriarRetornoSucesso(carga);
                }
                catch (ServicoException excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    unitOfWork.Rollback();

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>.CriarRetornoExcecao("Falha ao obter dados da carga.");
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>.CriarRetornoExcecao("Ocorreu uma falha ao consultar a Carga");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> ObterDadosCargasMotorista(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);

                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga> cargas = servicoFilaCarregamento.ObterDadosCargasMotorista(motorista.Codigo);

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>>.CriarRetornoSucesso(cargas);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as cargas do motorista");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido>> ObterPedidosPorCarga(string token, string usuario, string empresaMultisoftware, string codigoCarga)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);

                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido> pedidos = servicoFilaCarregamento.ObterPedidosPorCarga(codigoCarga.ToInt());

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido>>.CriarRetornoSucesso(pedidos);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Pedido>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os pedidos da carga selecionada");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo>> ObterFilasCarregamentoAguardandoChegadaVeiculo(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario usuarioEmbarcador = ObterUsuario(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo> filasCarregamento = servicoFilaCarregamento.ObterListaFilaCarregamentoAguardandoChegadaVeiculo(usuarioEmbarcador.Codigo);

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo>>.CriarRetornoSucesso(filasCarregamento);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.FilaCarregamentoVeiculo>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as filas de carregamento aguardando a confirmacao de chegada de veículo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento>> ObterMotivosRetiradaFilaCarregamento(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            var unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                var usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                var unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                var motivosRetiradaFilaCarregamento = ObterMotivosRetiradaFilaCarregamento(unitOfWork);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento>>.CriarRetornoSucesso(motivosRetiradaFilaCarregamento);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.MotivoRetiradaFilaCarregamento>>.CriarRetornoExcecao("Ocorreu uma falha ao buscar os motivos de retirada da fila de carregamento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao> ObterNotificacao(string token, int usuario, int empresaMultisoftware, int codigoNotificacao)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao notificacao = servicoFilaCarregamento.ObterNotificacao(codigoNotificacao);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao>.CriarRetornoSucesso(notificacao);
                }
                catch (ServicoException excecao)
                {
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.Notificacao>.CriarRetornoExcecao("Ocorreu uma falha ao obter a notificação");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>> ObterNotificacoes(string token, int usuario, int empresaMultisoftware, bool somenteNaoLidas)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    if (motorista == null)
                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>>.CriarRetornoSucesso(new List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>());

                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida> notificacoes = servicoFilaCarregamento.ObterNotificacoes(motorista.Codigo, somenteNaoLidas);

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>>.CriarRetornoSucesso(notificacoes);
                }
                catch (ServicoException excecao)
                {
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>>.CriarRetornoDadosInvalidos(excecao.Message);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.FilaCarregamento.NotificacaoResumida>>.CriarRetornoExcecao("Ocorreu uma falha ao obter as notificações");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga>> ObterTiposRetornoCarga(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga> tiposRetornoCarga = servicoFilaCarregamento.ObterListaTipoRetornoCarga();

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga>>.CriarRetornoSucesso(tiposRetornoCarga);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoRetornoCarga>>.CriarRetornoExcecao("Ocorreu uma falha ao obter os tipos de retorno de carga");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento>> ObterMotivosAtendimentos(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            var unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                var usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);
                var unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                var motivosAtendimento = ObterMotivosAtendimento(unitOfWork);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento>>.CriarRetornoSucesso(motivosAtendimento);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.MotivoAtendimento>>.CriarRetornoExcecao("Ocorreu uma falha ao buscar os motivos de atendimento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>> ObterAtendimentosCarga(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                try
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoMobile servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoMobile(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento> atendimentosCarga = ObterAtendimentosCarga(motorista.Codigo, unitOfWork);

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>>.CriarRetornoSucesso(atendimentosCarga);
                }
                catch (ServicoException excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    unitOfWork.Rollback();

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>>.CriarRetornoExcecao("Falha ao obter dados da carga.");
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar a Carga");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<string> ObterArquivoPoliticaPrivacidade(string token, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileClientePorCliente(empresaMultisoftware.ToInt(), unitOfWorkAdmin);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                try
                {
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    if (string.IsNullOrWhiteSpace(config.CaminhoArquivoPoliticaPrivacidadeMobile))
                        throw new WebServiceException("Arquivo ainda não foi anexado pelo Embarcador! Favor entrar em contato com o mesmo.");

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(config.CaminhoArquivoPoliticaPrivacidadeMobile))
                        throw new WebServiceException("Arquivo não foi salvo pelo Embarcador! Favor entrar em contato com o mesmo.");

                    byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(config.CaminhoArquivoPoliticaPrivacidadeMobile);

                    return Retorno<string>.CriarRetornoSucesso(Convert.ToBase64String(arquivoBinario));
                }
                catch (Exception)
                {
                    unitOfWork.Rollback();
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<string>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha ao obter o arquivo de Política de Privacidade");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion
    }
}
