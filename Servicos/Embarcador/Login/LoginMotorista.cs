using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Login
{
    public class LoginMotorista
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;

        public LoginMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async void AutenticarMotorista(string imagemBase64, int codigoCarga)
        {
            try
            {
                ReconhecimentoFacial.ReconhecimentoFacial reconhecimentoFacial = new ReconhecimentoFacial.ReconhecimentoFacial("", "", _unitOfWork);
                bool autenticado = await reconhecimentoFacial.AutenticarAsync(imagemBase64, codigoCarga, _unitOfWork);

                if (!autenticado)
                {
                    SalvarTentativaLogin(codigoCarga);
                    throw new ServicoException("O reconhecimento facial não foi validado.");
                }
            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.MotoristaSemImagemCadastrada)
            {
                EnviarEmailMotoristaSemImagemCadastrada(codigoCarga);
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        private void SalvarTentativaLogin(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Login.TentativaLoginMotorista repositorioTentativaLoginMotorista = new Repositorio.Embarcador.Login.TentativaLoginMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
            DateTime dataTentativa = DateTime.Now;

            Dominio.Entidades.Embarcador.Login.TentativaLoginMotorista tentativaLoginMotorista = new Dominio.Entidades.Embarcador.Login.TentativaLoginMotorista()
            {
                CodigoCarga = codigoCarga,
                CodigoMotorista = carga.Motoristas.FirstOrDefault().Codigo,
                Data = dataTentativa
            };

            repositorioTentativaLoginMotorista.Inserir(tentativaLoginMotorista);

            int quantidadeTentativas = repositorioTentativaLoginMotorista.BuscarQuantidadeTentativasPorDataCargaMotorista(dataTentativa, codigoCarga, carga.Motoristas.FirstOrDefault().Codigo);

            try
            {
                if (quantidadeTentativas >= 3)
                    EnviarEmailNumeroTentativasFalhasFilial(carga);
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        private void EnviarEmailMotoristaSemImagemCadastrada(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            string emailFilial = carga.Filial?.Email;

            if (string.IsNullOrWhiteSpace(emailFilial))
                throw new ServicoException("A filial está nula ou não possui e-mail cadastrado.");

            string bodyEmail = ObterBodyDadosEmbarque(carga);

            bodyEmail = $"{bodyEmail}{Environment.NewLine}Motivo da rejeição: Motorista sem imagem na base de dados da Agrária.";

            Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = new Repositorio.ConfiguracaoEmail(_unitOfWork).BuscarConfiguracao();

            if (configuracaoEmail == null)
                throw new ServicoException("Não existe configuração de e-mail.");

            Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailFilial, null, null, "Alerta - Login Motorista", bodyEmail, configuracaoEmail.Smtp, out string mensagemErro, "", null, "", false, "", configuracaoEmail.PortaSmtp, _unitOfWork);
        }

        private void EnviarEmailNumeroTentativasFalhasFilial(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string emailFilial = carga.Filial?.Email;

            if (string.IsNullOrWhiteSpace(emailFilial))
                throw new ServicoException("A filial está nula ou não possui e-mail cadastrado.");

            string bodyEmail = ObterBodyDadosEmbarque(carga);

            bodyEmail = $"{bodyEmail}{Environment.NewLine}Motivo da rejeição: 3 tentativas de reconhecimento facial inválidas.";

            Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = new Repositorio.ConfiguracaoEmail(_unitOfWork).BuscarConfiguracao();

            if (configuracaoEmail == null)
                throw new ServicoException("Não existe configuração de e-mail.");

            Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, emailFilial, null, null, "Alerta - Login Motorista", bodyEmail, configuracaoEmail.Smtp, out string mensagemErro, "", null, "", false, "", configuracaoEmail.PortaSmtp, _unitOfWork);
        }

        private string ObterBodyDadosEmbarque(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            IEnumerable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = carga.Pedidos;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            StringBuilder bodySB = new StringBuilder($"Dados do embarque:{Environment.NewLine}");

            bodySB.Append(Environment.NewLine);
            bodySB.AppendLine($"Número da carga: {carga.CodigoCargaEmbarcador}");
            bodySB.AppendLine($"Dados do Motorista: {carga.Motoristas.FirstOrDefault().Descricao}");
            bodySB.AppendLine($"Veículo: {carga.Veiculo.Placa_Formatada}");
            bodySB.AppendLine($"Destino: {cargasPedidos.LastOrDefault().Pedido.Destinatario?.Localidade?.Descricao}");
            bodySB.AppendLine($"Peso: {carga.DadosSumarizados?.PesoTotal.ToString("n2")}");
            bodySB.AppendLine($"Horário de Carregamento: {cargaJanelaCarregamento.DataCarregamentoProgramada.ToString("dd/MM/yyyy HH:mm")}");

            return bodySB.ToString();
        }
    }
}
