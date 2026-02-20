using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.WebService.Frota
{
    public class Infracao
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public Infracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AdicionarMulta(Dominio.ObjetosDeValor.WebService.Frota.Multa multaIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Log.TratarErro($"AdicionarMulta: {Newtonsoft.Json.JsonConvert.SerializeObject(multaIntegracao)}");

            if (string.IsNullOrWhiteSpace(multaIntegracao.event_type) || !multaIntegracao.event_type.Equals("frame"))
                throw new ServicoException("O tipo (eventType) deve ser Multa (frame)");

            if (multaIntegracao.frame == null || multaIntegracao.car == null || multaIntegracao.driver == null || multaIntegracao.organization == null)
                throw new ServicoException("O arquivo JSON enviado está fora do padrão de uma Multa");

            Repositorio.Embarcador.Frota.Infracao repositorioInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
            Repositorio.Embarcador.Frota.TipoInfracao repositorioTipoInfracao = new Repositorio.Embarcador.Frota.TipoInfracao(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista repHistoricoVeiculoVinculoMotorista = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista(_unitOfWork);

            int codigoOrgaoEmissor = multaIntegracao.organization.organization_code;
            int codigoIBGECidade = multaIntegracao.frame.city_code.ToInt();
            string descricaoCidade = multaIntegracao.frame.city;
            string ufCidade = multaIntegracao.frame.state;
            string codigoMulta = multaIntegracao.frame.code;
            string placaVeiculo = multaIntegracao.car.car_plate;
            string cpfMotorista = multaIntegracao.driver.tax_id;

            Dominio.Entidades.Cliente orgaoEmissor = codigoOrgaoEmissor > 0 ? repositorioCliente.BuscarPorCodigoIntegracao(codigoOrgaoEmissor.ToString()) : null;

            Dominio.Entidades.Localidade cidade = codigoIBGECidade > 0 ? repositorioLocalidade.BuscarPorCodigoIBGE(codigoIBGECidade) : null;
            if (cidade == null)
                cidade = repositorioLocalidade.BuscarPorDescricaoEUF(descricaoCidade, ufCidade);
            if (cidade == null)
                cidade = codigoIBGECidade > 0 ? repositorioLocalidade.buscarPorCodigoEmbarcador(codigoIBGECidade.ToString()) : null;
            if (cidade == null)
                cidade = codigoIBGECidade > 0 ? repositorioLocalidade.buscarPorCodigoDocumento(codigoIBGECidade.ToString()) : null;

            Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = repositorioTipoInfracao.BuscarPorCodigoCTB(codigoMulta);
            if (tipoInfracao == null)
                throw new ServicoException($"Tipo da infração não foi encontrada. Código CTB recebido {codigoMulta}");

            Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(placaVeiculo);
            if (veiculo == null)
                throw new ServicoException($"Veículo placa {placaVeiculo} não foi encontrado");

            DateTime data = $"{multaIntegracao.frame.at} {multaIntegracao.frame.time ?? "00:00"}".ToNullableDateTime("yyyy-MM-dd HH:mm:ss") ?? throw new ServicoException("Data/Hora é obrigatória.");

            Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarPorCPF(cpfMotorista);
            if (motorista == null && veiculo != null)
            {
                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoMotorista historicoVeiculoVinculoMotorista = repHistoricoVeiculoVinculoMotorista.BuscarMotoristaPorVeiculo(veiculo.Codigo, data);

                if (historicoVeiculoVinculoMotorista?.Motorista != null)
                    motorista = historicoVeiculoVinculoMotorista.Motorista;
                else
                    motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
            }

            string numeroAtuacao = multaIntegracao.frame.ait;
            if (!string.IsNullOrWhiteSpace(numeroAtuacao))
                if (repositorioInfracao.ContemInfracaoMesmoNumeroAtuacao(numeroAtuacao, 0))
                    throw new ServicoException("Já existe uma infração cadastrada com este mesmo Número de Atuação.");

            string arquivoIntegracaoJson = Newtonsoft.Json.JsonConvert.SerializeObject(multaIntegracao);

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Frota.Infracao infracao = new Dominio.Entidades.Embarcador.Frota.Infracao()
            {
                Situacao = SituacaoInfracao.AguardandoConfirmacaoIntegracao,
                DataLancamento = DateTime.Now,
                Numero = repositorioInfracao.BuscarProximoNumero(),
                TipoOcorrenciaInfracao = TipoOcorrenciaInfracao.Veiculo,
                NumeroAtuacao = numeroAtuacao,
                Data = data,
                DataEmissaoInfracao = multaIntegracao.created_at.ToNullableDateTime("yyyy-MM-ddTHH:mm:ss"),
                DataLimiteIndicacaoCondutor = multaIntegracao.frame.expiration_at.ToNullableDateTime("yyyy-MM-dd HH:mm:ss") ?? multaIntegracao.driver.indicate.ToNullableDateTime("yyyy-MM-ddTHH:mm:ss"),
                Local = multaIntegracao.frame.address,
                OrgaoEmissor = orgaoEmissor,
                Cidade = cidade,
                TipoInfracao = tipoInfracao,
                Veiculo = veiculo,
                Motorista = motorista,
                ArquivoIntegracao = arquivoIntegracaoJson,

                ProdutoCarga = string.Empty,
                CausaSinistro = string.Empty,
                ClassificacaoSinistro = ClassificacaoSinistro.Sl,
                LancarDescontoMotorista = tipoInfracao.LancarDescontoMotorista,
                DescontoComissaoMotorista = tipoInfracao.DescontoComissaoMotorista,
                JustificativaDesconto = tipoInfracao.JustificativaDesconto,
                ReduzirPercentualComissaoMotorista = tipoInfracao.ReduzirPercentualComissaoMotorista,
                PercentualReducaoComissaoMotorista = tipoInfracao.PercentualReducaoComissaoMotorista
            };

            repositorioInfracao.Inserir(infracao, auditado);

            _unitOfWork.CommitChanges();

            Log.TratarErro($"Finalizou o AdicionarMulta");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AdicionarNotificacao(Dominio.ObjetosDeValor.WebService.Frota.Notificacao notificacaoIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Log.TratarErro($"AdicionarNotificacao: {Newtonsoft.Json.JsonConvert.SerializeObject(notificacaoIntegracao)}");

            if (string.IsNullOrWhiteSpace(notificacaoIntegracao.event_type) || !notificacaoIntegracao.event_type.Equals("notification"))
                throw new ServicoException("O tipo (eventType) deve ser Notificação (notification)");

            if (notificacaoIntegracao.notification == null || notificacaoIntegracao.car == null || notificacaoIntegracao.driver == null || notificacaoIntegracao.organization == null)
                throw new ServicoException("O arquivo JSON enviado está fora do padrão de uma Notificação");

            Repositorio.Embarcador.Frota.Infracao repositorioInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
            Repositorio.Embarcador.Frota.TipoInfracao repositorioTipoInfracao = new Repositorio.Embarcador.Frota.TipoInfracao(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            int codigoOrgaoEmissor = notificacaoIntegracao.organization.organization_code;
            int codigoIBGECidade = notificacaoIntegracao.notification.city_code.ToInt();
            string descricaoCidade = notificacaoIntegracao.notification.city;
            string ufCidade = notificacaoIntegracao.notification.state;
            string codigoMulta = notificacaoIntegracao.notification.code;
            string placaVeiculo = notificacaoIntegracao.car.car_plate;
            string cpfMotorista = notificacaoIntegracao.driver.tax_id;

            Dominio.Entidades.Cliente orgaoEmissor = codigoOrgaoEmissor > 0 ? repositorioCliente.BuscarPorCodigoIntegracao(codigoOrgaoEmissor.ToString()) : null;

            Dominio.Entidades.Localidade cidade = codigoIBGECidade > 0 ? repositorioLocalidade.BuscarPorCodigoIBGE(codigoIBGECidade) : null;
            if (cidade == null)
                cidade = repositorioLocalidade.BuscarPorDescricaoEUF(descricaoCidade, ufCidade);
            if (cidade == null)
                cidade = codigoIBGECidade > 0 ? repositorioLocalidade.buscarPorCodigoEmbarcador(codigoIBGECidade.ToString()) : null;
            if (cidade == null)
                cidade = codigoIBGECidade > 0 ? repositorioLocalidade.buscarPorCodigoDocumento(codigoIBGECidade.ToString()) : null;

            Dominio.Entidades.Embarcador.Frota.TipoInfracao tipoInfracao = repositorioTipoInfracao.BuscarPorCodigoCTB(codigoMulta);
            if (tipoInfracao == null)
                throw new ServicoException($"Tipo da infração não foi encontrada. Código CTB recebido {codigoMulta}");

            Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(placaVeiculo);
            if (veiculo == null)
                throw new ServicoException($"Veículo placa {placaVeiculo} não foi encontrado");

            Dominio.Entidades.Usuario motorista = repositorioUsuario.BuscarPorCPF(cpfMotorista);
            if (motorista == null && veiculo != null)
                motorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            DateTime data = $"{notificacaoIntegracao.notification.at} {notificacaoIntegracao.notification.time ?? "00:00"}".ToNullableDateTime("yyyy-MM-dd HH:mm:ss") ?? throw new ServicoException("Data/Hora é obrigatória.");

            string numeroAtuacao = notificacaoIntegracao.notification.ait;
            if (!string.IsNullOrWhiteSpace(numeroAtuacao))
                if (repositorioInfracao.ContemInfracaoMesmoNumeroAtuacao(numeroAtuacao, 0))
                    throw new ServicoException("Já existe uma infração cadastrada com este mesmo Número de Atuação.");

            string arquivoIntegracaoJson = Newtonsoft.Json.JsonConvert.SerializeObject(notificacaoIntegracao);

            _unitOfWork.Start();

            Dominio.Entidades.Embarcador.Frota.Infracao infracao = new Dominio.Entidades.Embarcador.Frota.Infracao()
            {
                Situacao = SituacaoInfracao.AguardandoConfirmacaoIntegracao,
                DataLancamento = DateTime.Now,
                Numero = repositorioInfracao.BuscarProximoNumero(),
                TipoOcorrenciaInfracao = TipoOcorrenciaInfracao.Veiculo,
                NumeroAtuacao = numeroAtuacao,
                Data = data,
                DataEmissaoInfracao = notificacaoIntegracao.created_at.ToNullableDateTime("yyyy-MM-ddTHH:mm:ss"),
                DataLimiteIndicacaoCondutor = notificacaoIntegracao.notification.indication_limit_at.ToNullableDateTime("yyyy-MM-dd HH:mm:ss") ?? notificacaoIntegracao.driver.indicate.ToNullableDateTime("yyyy-MM-ddTHH:mm:ss"),
                Local = notificacaoIntegracao.notification.address,
                OrgaoEmissor = orgaoEmissor,
                Cidade = cidade,
                TipoInfracao = tipoInfracao,
                Veiculo = veiculo,
                Motorista = motorista,
                ArquivoIntegracao = arquivoIntegracaoJson,

                ProdutoCarga = string.Empty,
                CausaSinistro = string.Empty,
                ClassificacaoSinistro = ClassificacaoSinistro.Sl,
                LancarDescontoMotorista = tipoInfracao.LancarDescontoMotorista,
                DescontoComissaoMotorista = tipoInfracao.DescontoComissaoMotorista,
                JustificativaDesconto = tipoInfracao.JustificativaDesconto,
                ReduzirPercentualComissaoMotorista = tipoInfracao.ReduzirPercentualComissaoMotorista,
                PercentualReducaoComissaoMotorista = tipoInfracao.PercentualReducaoComissaoMotorista
            };

            repositorioInfracao.Inserir(infracao, auditado);

            _unitOfWork.CommitChanges();

            Log.TratarErro($"Finalizou o AdicionarNotificacao");

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        #endregion Métodos Públicos
    }
}
