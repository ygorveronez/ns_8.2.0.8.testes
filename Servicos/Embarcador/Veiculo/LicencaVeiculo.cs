using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Veiculo
{
    public sealed class LicencaVeiculo
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos

        #region Construtores

        public LicencaVeiculo(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AdicionarFaixasTemperatura(Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca, List<string> faixasTemperaturaSalvar)
        {
            licenca.FaixasTemperatura?.Clear();

            if (faixasTemperaturaSalvar.Count == 0)
                return;

            if (licenca.FaixasTemperatura == null)
                licenca.FaixasTemperatura = new List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

            List<int> codigosFaixasTemperatura = (from o in faixasTemperaturaSalvar select o.ToInt()).ToList();
            Repositorio.Embarcador.Cargas.FaixaTemperatura repositorioFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = repositorioFaixaTemperatura.BuscarPorCodigos(codigosFaixasTemperatura);

            foreach (Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura in faixasTemperatura)
                licenca.FaixasTemperatura.Add(faixaTemperatura);
        }

        private void AtualizarCargasLicenca(Dominio.Entidades.Veiculo veiculo, DateTime dataInicial, DateTime dataFinal)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarCargaDePreCargaEmAbertoPorVeiculo(veiculo.Codigo, dataInicial, dataFinal);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                GerarCargaLicenca(carga);
        }

        private List<Dominio.Entidades.Veiculo> ObterVeiculosGerarCargaLicenca(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

            if (!(carga.TipoOperacao?.ValidarLicencaVeiculo ?? false))
                return veiculos;

            if (!(carga.TipoDeCarga?.ControlaTemperatura ?? false))
                return veiculos;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            if (configuracaoGeralCarga.NaoValidarLicencaVeiculoParaCargaRedespacho && (carga.Redespacho != null))
                return veiculos;

            if (carga.Veiculo?.ModeloVeicularCarga?.ValidarLicencaVeiculo ?? false)
                veiculos.Add(carga.Veiculo);

            if ((carga.VeiculosVinculados != null) && carga.VeiculosVinculados.Any(o => o.ModeloVeicularCarga?.ValidarLicencaVeiculo ?? false))
                veiculos.AddRange(carga.VeiculosVinculados.Where(o => o.ModeloVeicularCarga?.ValidarLicencaVeiculo ?? false));

            return veiculos;
        }

        private async Task<List<Dominio.Entidades.Veiculo>> ObterVeiculosGerarCargaLicencaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

            if (!(carga.TipoOperacao?.ValidarLicencaVeiculo ?? false))
                return veiculos;

            if (!(carga.TipoDeCarga?.ControlaTemperatura ?? false))
                return veiculos;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();

            if (configuracaoGeralCarga.NaoValidarLicencaVeiculoParaCargaRedespacho && (carga.Redespacho != null))
                return veiculos;

            if (carga.Veiculo?.ModeloVeicularCarga?.ValidarLicencaVeiculo ?? false)
                veiculos.Add(carga.Veiculo);

            if ((carga.VeiculosVinculados != null) && carga.VeiculosVinculados.Any(o => o.ModeloVeicularCarga?.ValidarLicencaVeiculo ?? false))
                veiculos.AddRange(carga.VeiculosVinculados.Where(o => o.ModeloVeicularCarga?.ValidarLicencaVeiculo ?? false));

            return veiculos;
        }

        private void Remover(List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasExistentesNaoUtilizadas, List<int> codigosLicencaAdicionadasOuAtualizadas, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);
            Anexo.Anexo<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculoAnexo, Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> servicoLicencaAnexo = new Anexo.Anexo<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculoAnexo, Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasDeletar = licencasExistentesNaoUtilizadas.Where(o => !codigosLicencaAdicionadasOuAtualizadas.Contains(o.Codigo)).ToList();

            foreach (Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca in licencasDeletar)
            {
                servicoLicencaAnexo.ExcluirAnexos(licenca);
                repositorioLicencaVeiculo.Deletar(licenca);

                Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, $"Removido licença {licenca.Licenca?.Descricao ?? ""}, Vencimento {licenca.DataVencimento?.ToString("dd/MM/yyyy") ?? ""}.", _unitOfWork);

            }
        }

        private async Task ValidarLicencasAsync(List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculo, Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = await repositorioCargaJanelaCarregamento.BuscarPorCargaAsync(cargaLicenca.Carga.Codigo);

            List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculoNaoVencidas = licencasVeiculo.Where(obj => obj.DataVencimento >= cargaJanelaCarregamento?.InicioCarregamento.Date).ToList();

            bool todasValidas = !(licencasVeiculoNaoVencidas.Count == 0);

            if (todasValidas)
            {
                licencasVeiculoNaoVencidas = licencasVeiculoNaoVencidas.Where(obj => obj.Status == StatusLicenca.Aprovado).ToList();
                todasValidas = !(licencasVeiculoNaoVencidas.Count == 0);
            }

            if (todasValidas)
            {
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licencaAprovada = licencasVeiculoNaoVencidas.OrderByDescending(obj => obj.DataVencimento).FirstOrDefault();
                cargaLicenca.Mensagem = string.Empty;
                cargaLicenca.Situacao = EnumSituacaoCargaLicenca.Valida;
                cargaLicenca.LicencaVeiculo = licencaAprovada;
            }
            else
            {
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licencaVeiculoMensagem = licencasVeiculo.OrderByDescending(obj => obj.DataVencimento).FirstOrDefault();
                if (licencaVeiculoMensagem.DataVencimento < cargaJanelaCarregamento?.InicioCarregamento.Date)
                {
                    cargaLicenca.Mensagem = _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador ? cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaVencidaEmbarcador : cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaVencidaTransportador;
                    cargaLicenca.Situacao = EnumSituacaoCargaLicenca.Vencida;
                    cargaLicenca.LicencaVeiculo = licencaVeiculoMensagem;
                }
                else
                {
                    cargaLicenca.Mensagem = _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador ? cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaReprovadaEmbarcador : cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaReprovadaTransportador;
                    cargaLicenca.Situacao = EnumSituacaoCargaLicenca.Reprovada;
                    cargaLicenca.LicencaVeiculo = licencaVeiculoMensagem;
                }
            }
        }
        private void ValidarLicencas(List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculo, Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(cargaLicenca.Carga.Codigo);

            List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculoNaoVencidas = licencasVeiculo.Where(obj => obj.DataVencimento >= cargaJanelaCarregamento?.InicioCarregamento.Date).ToList();

            bool todasValidas = !(licencasVeiculoNaoVencidas.Count == 0);

            if (todasValidas)
            {
                licencasVeiculoNaoVencidas = licencasVeiculoNaoVencidas.Where(obj => obj.Status == StatusLicenca.Aprovado).ToList();
                todasValidas = !(licencasVeiculoNaoVencidas.Count == 0);
            }

            if (todasValidas)
            {
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licencaAprovada = licencasVeiculoNaoVencidas.OrderByDescending(obj => obj.DataVencimento).FirstOrDefault();
                cargaLicenca.Mensagem = string.Empty;
                cargaLicenca.Situacao = EnumSituacaoCargaLicenca.Valida;
                cargaLicenca.LicencaVeiculo = licencaAprovada;
            }
            else
            {
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licencaVeiculoMensagem = licencasVeiculo.OrderByDescending(obj => obj.DataVencimento).FirstOrDefault();
                if (licencaVeiculoMensagem.DataVencimento < cargaJanelaCarregamento?.InicioCarregamento.Date)
                {
                    cargaLicenca.Mensagem = _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador ? cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaVencidaEmbarcador : cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaVencidaTransportador;
                    cargaLicenca.Situacao = EnumSituacaoCargaLicenca.Vencida;
                    cargaLicenca.LicencaVeiculo = licencaVeiculoMensagem;
                }
                else
                {
                    cargaLicenca.Mensagem = _tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador ? cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaReprovadaEmbarcador : cargaLicenca.Carga.TipoDeCarga?.FaixaDeTemperatura?.MensagemLicencaReprovadaTransportador;
                    cargaLicenca.Situacao = EnumSituacaoCargaLicenca.Reprovada;
                    cargaLicenca.LicencaVeiculo = licencaVeiculoMensagem;
                }
            }
        }

        private void ValidarCargaLicenca(Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca, bool origemCadastro)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (cargaLicenca != null)
            {
                cargaLicenca.Carga.LicencaInvalida = !cargaLicenca.Situacao.Isvalida();
                cargaLicenca.Carga.AguardandoSalvarDadosTransporteCarga = origemCadastro;
                repositorioCarga.Atualizar(cargaLicenca.Carga);
            }
        }

        private async Task ValidarCargaLicencaAsync(Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca, bool origemCadastro)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (cargaLicenca != null)
            {
                cargaLicenca.Carga.LicencaInvalida = !cargaLicenca.Situacao.Isvalida();
                cargaLicenca.Carga.AguardandoSalvarDadosTransporteCarga = origemCadastro;
                await repositorioCarga.AtualizarAsync(cargaLicenca.Carga);
            }
        }

        private void ValidarRegrasCadastro(Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca, bool validarDuplicados)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);

            if (licenca.DataEmissao > licenca.DataVencimento)
                throw new ServicoException(Localization.Resources.Veiculos.VeiculoLicenca.DataEmissaoMaiorDataVencimento);

            if (validarDuplicados && _tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS && repositorioLicencaVeiculo.ContemLicencaVeiculoDuplicada(licenca.Codigo, licenca.Veiculo.Codigo, licenca.Licenca?.Codigo ?? 0, licenca.Filial?.Codigo ?? 0, licenca.DataEmissao.Value, licenca.DataVencimento.Value))
                throw new ServicoException(Localization.Resources.Veiculos.VeiculoLicenca.LicencaDuplicada);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo AdicionarOuAtualizar(Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar licencaSalvar, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            return AdicionarOuAtualizar(veiculo, licencaSalvar, auditado, true);
        }

        public Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo AdicionarOuAtualizar(Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar licencaSalvar, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool validarDuplicados)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU repositorioClassificacaoRiscoONU = new Repositorio.Embarcador.Pedidos.ClassificacaoRiscoONU(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Licenca repositorioLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(_unitOfWork);

            int codigoClassificacaoRiscoONU = licencaSalvar.ClassificacaoRiscoONU.ToInt();
            int codigoLicenca = licencaSalvar.Licenca.ToInt();
            int codigoLicencaVeiculo = licencaSalvar.Codigo.ToInt();
            int codigoFilial = licencaSalvar.Filial.ToInt();
            int codigoTipoCarga = licencaSalvar.TipoCarga.ToInt();
            int codigoContainer = licencaSalvar.Container.ToInt();
            dynamic formasAlerta = JsonConvert.DeserializeObject<dynamic>(licencaSalvar.FormaAlerta);
            List<string> faixasTemperatura = JsonConvert.DeserializeObject<List<string>>(licencaSalvar.FaixaTemperatura);

            Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca = codigoLicencaVeiculo > 0 ? repositorioLicencaVeiculo.BuscarPorCodigo(codigoLicencaVeiculo, true) : new Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo();

            licenca.BloquearCriacaoPedidoLicencaVencida = licencaSalvar.BloquearCriacaoPedidoLicencaVencida.ToBool();
            licenca.ClassificacaoRiscoONU = codigoClassificacaoRiscoONU > 0 ? repositorioClassificacaoRiscoONU.BuscarPorCodigo(codigoClassificacaoRiscoONU) : null;
            licenca.DataEmissao = licencaSalvar.DataEmissao.ToDateTime();
            licenca.DataVencimento = licencaSalvar.DataVencimento.ToDateTime();
            licenca.Descricao = licencaSalvar.Descricao;
            licenca.Licenca = codigoLicenca > 0 ? repositorioLicenca.BuscarPorCodigo(codigoLicenca) : null;
            licenca.Numero = licencaSalvar.Numero;
            licenca.Veiculo = veiculo;
            licenca.Status = licencaSalvar.StatusLicenca.ToEnum<StatusLicenca>();
            licenca.Vencido = licenca.DataVencimento < DateTime.Now.Date ? true : false;
            licenca.Filial = codigoFilial > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilial) : null;
            licenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida = licencaSalvar.BloquearCriacaoPlanejamentoPedidoLicencaVencida.ToBool();
            licenca.NumeroContainer = licencaSalvar.NumeroContainer;
            licenca.TipoCarga = codigoTipoCarga > 0 ? repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoCarga) : null;
            licenca.Container = codigoContainer > 0 ? repositorioContainer.BuscarPorCodigo(codigoContainer) : null;

            licenca.FormasAlerta = new List<ControleAlertaForma>();
            if (formasAlerta?.Count > 0)
            {
                foreach (var codigoFormaAlerta in formasAlerta)
                    licenca.FormasAlerta.Add(((string)codigoFormaAlerta).ToEnum<ControleAlertaForma>());
            }

            AdicionarFaixasTemperatura(licenca, faixasTemperatura);

            licenca.UsuarioAlteracao = auditado.Usuario;
            licenca.DataAlteracao = DateTime.Now;

            ValidarRegrasCadastro(licenca, validarDuplicados);

            if (licenca.Codigo > 0)
                repositorioLicencaVeiculo.Atualizar(licenca, auditado);
            else
                repositorioLicencaVeiculo.Inserir(licenca, auditado);

            AtualizarCargasLicenca(veiculo, licenca.DataEmissao.Value, licenca.DataVencimento.Value);

            return licenca;
        }

        public void AdicionarOuAtualizar(Dominio.Entidades.Veiculo veiculo, List<Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar> listaLicencaSalvar, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool validarDuplicados)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasExistentesNaoUtilizadas = repositorioLicencaVeiculo.BuscarNaoUtilizadasPorVeiculo(veiculo.Codigo);
            List<int> codigosLicencaAdicionadasOuAtualizadas = new List<int>();
            DateTime? dataInicial = null;
            DateTime? dataFinal = null;

            foreach (Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar licencaSalvar in listaLicencaSalvar)
            {
                Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca = AdicionarOuAtualizar(veiculo, licencaSalvar, auditado, validarDuplicados);
                codigosLicencaAdicionadasOuAtualizadas.Add(licenca.Codigo);

                if ((!dataInicial.HasValue || dataInicial > licenca.DataEmissao.Value) && !licenca.Vencido)
                    dataInicial = licenca.DataEmissao.Value;
                if ((!dataFinal.HasValue || dataFinal < licenca.DataVencimento.Value) && !licenca.Vencido)
                    dataFinal = licenca.DataVencimento.Value;
            }

            Remover(licencasExistentesNaoUtilizadas, codigosLicencaAdicionadasOuAtualizadas, auditado, veiculo);

            if (dataInicial.HasValue && dataFinal.HasValue)
                AtualizarCargasLicenca(veiculo, dataInicial.Value, dataFinal.Value);
        }

        public void GerarCargaLicenca(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool origemCadastro = false)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(_unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca = repositorioCargaLicenca.BuscarPorCarga(carga.Codigo);
            if (cargaLicenca != null)
                repositorioCargaLicenca.Deletar(cargaLicenca);

            List<Dominio.Entidades.Veiculo> veiculos = ObterVeiculosGerarCargaLicenca(carga);
            if (veiculos.Count == 0)
                return;

            int codigoContainer = repositorioRetiradaContainer.BuscarCodigoContainerPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculo = repositorioLicencaVeiculo.BuscarPorVeiculoTipoDeCarga(veiculo.Placa, carga.TipoDeCarga?.FaixaDeTemperatura?.Codigo ?? 0, carga.TipoDeCarga?.Codigo ?? 0, codigoContainer, carga.TipoOperacao.ValidarLicencaVeiculoPorCarga);

                cargaLicenca = new Dominio.Entidades.Embarcador.Cargas.CargaLicenca()
                {
                    DataValidacao = DateTime.Now,
                    LicencaVeiculo = null,
                    Mensagem = string.Empty,
                    Situacao = EnumSituacaoCargaLicenca.Nenhuma,
                    Carga = carga
                };

                if (licencasVeiculo.Count == 0)
                {
                    cargaLicenca.Mensagem = $"Nenhuma licença cadastrada/vigente para o veículo {veiculo.Placa}.{(carga.TipoOperacao.ValidarLicencaVeiculoPorCarga ? " Tipo de operação valida se licenças estão em outras cargas também!" : "")}";
                    cargaLicenca.Situacao = EnumSituacaoCargaLicenca.SemLicenca;

                    repositorioCargaLicenca.Inserir(cargaLicenca);

                    ValidarCargaLicenca(cargaLicenca, origemCadastro);

                    return;
                }

                ValidarLicencas(licencasVeiculo, cargaLicenca);
                ValidarCargaLicenca(cargaLicenca, origemCadastro);

                if (cargaLicenca.Situacao != EnumSituacaoCargaLicenca.Valida)
                    break;
            }

            repositorioCargaLicenca.Inserir(cargaLicenca);
        }

        public async Task GerarCargaLicencaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool origemCadastro = false)
        {
            Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(_unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca = await repositorioCargaLicenca.BuscarPorCargaAsync(carga.Codigo);
            if (cargaLicenca != null)
                await repositorioCargaLicenca.DeletarAsync(cargaLicenca);

            List<Dominio.Entidades.Veiculo> veiculos = await ObterVeiculosGerarCargaLicencaAsync(carga);
            if (veiculos.Count == 0)
                return;

            int codigoContainer = await repositorioRetiradaContainer.BuscarCodigoContainerPorCargaAsync(carga.Codigo);

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencasVeiculo = await repositorioLicencaVeiculo.BuscarPorVeiculoTipoDeCargaAsync(veiculo.Placa, carga.TipoDeCarga?.FaixaDeTemperatura?.Codigo ?? 0, carga.TipoDeCarga?.Codigo ?? 0, codigoContainer, carga.TipoOperacao.ValidarLicencaVeiculoPorCarga);

                cargaLicenca = new Dominio.Entidades.Embarcador.Cargas.CargaLicenca()
                {
                    DataValidacao = DateTime.Now,
                    LicencaVeiculo = null,
                    Mensagem = string.Empty,
                    Situacao = EnumSituacaoCargaLicenca.Nenhuma,
                    Carga = carga
                };

                if (licencasVeiculo.Count == 0)
                {
                    cargaLicenca.Mensagem = $"Nenhuma licença cadastrada/vigente para o veículo {veiculo.Placa}.{(carga.TipoOperacao.ValidarLicencaVeiculoPorCarga ? " Tipo de operação valida se licenças estão em outras cargas também!" : "")}";
                    cargaLicenca.Situacao = EnumSituacaoCargaLicenca.SemLicenca;

                    await repositorioCargaLicenca.InserirAsync(cargaLicenca);

                    await ValidarCargaLicencaAsync(cargaLicenca, origemCadastro);

                    return;
                }

                await ValidarLicencasAsync(licencasVeiculo, cargaLicenca);
                await ValidarCargaLicencaAsync(cargaLicenca, origemCadastro);

                if (cargaLicenca.Situacao != EnumSituacaoCargaLicenca.Valida)
                    break;
            }

            await repositorioCargaLicenca.InserirAsync(cargaLicenca);
        }

        public dynamic ObterDetalhes(Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca)
        {
            return new
            {
                licenca.Codigo,
                licenca.BloquearCriacaoPedidoLicencaVencida,
                licenca.BloquearCriacaoPlanejamentoPedidoLicencaVencida,
                DataEmissao = licenca.DataEmissao.Value.ToString("dd/MM/yyyy"),
                DataVencimento = licenca.DataVencimento.Value.ToString("dd/MM/yyyy"),
                licenca.Descricao,
                FormaAlerta = licenca.FormasAlerta.ToList(),
                licenca.Numero,
                StatusLicenca = licenca.Status,
                licenca.Vencido,
                licenca.NumeroContainer,
                ClassificacaoRiscoONU = new { Codigo = licenca.ClassificacaoRiscoONU?.Codigo ?? 0, Descricao = licenca.ClassificacaoRiscoONU?.Descricao ?? "" },
                Licenca = new { Codigo = licenca.Licenca?.Codigo ?? 0, Descricao = licenca.Licenca?.Descricao ?? "" },
                Veiculo = new { licenca.Veiculo.Codigo, licenca.Veiculo.Descricao },
                Filial = new { Codigo = licenca.Filial?.Codigo ?? 0, Descricao = licenca.Filial?.Descricao ?? "" },
                TipoCarga = new { Codigo = licenca.TipoCarga?.Codigo ?? 0, Descricao = licenca.TipoCarga?.Descricao ?? "" },
                Container = new { Codigo = licenca.Container?.Codigo ?? 0, Descricao = licenca.Container?.Descricao ?? "" },
                ExibirSelecaoContainer = licenca.Veiculo.ModeloVeicularCarga?.ContainerTipo != null,
                ListaFaixaTemperatura = (
                    from o in licenca.FaixasTemperatura
                    select new
                    {
                        o.Codigo,
                        o.Descricao
                    }
                ).ToList()
            };
        }

        public void ValidarCargaLicenca(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.Entidades.Veiculo> veiculos = ObterVeiculosGerarCargaLicenca(carga);

            if (veiculos.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaLicenca cargaLicenca = repositorioCargaLicenca.BuscarPorCarga(carga.Codigo);

            if ((cargaLicenca == null) || !cargaLicenca.Situacao.Isvalida())
            {
                string mensagem = cargaLicenca?.Mensagem ?? string.Empty;
                throw new ServicoException(string.IsNullOrWhiteSpace(mensagem) ? "Licença inválida" : mensagem, CodigoExcecao.LicencaInvalida);
            }
        }

        public void ValidarLicencasVeiculoVencidas()
        {
            if (_tipoServicoMultisoftware != TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Veiculos.LicencaVeiculo repositorioLicencaVeiculo = new Repositorio.Embarcador.Veiculos.LicencaVeiculo(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo> licencas = repositorioLicencaVeiculo.BuscarLicencasVencidas(DateTime.Today);

            foreach (Dominio.Entidades.Embarcador.Veiculos.LicencaVeiculo licenca in licencas)
            {
                licenca.Vencido = true;
                repositorioLicencaVeiculo.Atualizar(licenca);
            }
        }

        #endregion Métodos Públicos
    }
}
