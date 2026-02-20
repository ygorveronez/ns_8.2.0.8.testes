using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.WebService;
using System.Linq;

namespace Servicos.WebService.Abastecimento
{
    public class Abastecimento
    {
        #region Propriedades Privadas

        readonly private Repositorio.UnitOfWork _unitOfWork;
        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        readonly private AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        readonly protected string _adminStringConexao;

        #endregion

        #region Construtores

        public Abastecimento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public Retorno<bool> AtualizarStatusAbastecimentoConecttec(Dominio.ObjetosDeValor.WebService.Abastecimento.AtualizarStatusAbastecimentoConecttec atualizarStatusAbastecimentoConecttec)
        {
            Servicos.Log.TratarErro($"AtualizarStatusAbastecimentoConecttec: {Newtonsoft.Json.JsonConvert.SerializeObject(atualizarStatusAbastecimentoConecttec)}");

            try
            {
                if (atualizarStatusAbastecimentoConecttec == null)
                    throw new ServicoException("É obrigatório enviar os objetos da requisição.");

                if(!new Servicos.Embarcador.Integracao.Conecttec.IntegracaoConecttec(_unitOfWork).AtualizarStatusAbastecimento(atualizarStatusAbastecimentoConecttec))
                    throw new ServicoException("Erro ao atualizar status do abastecimento Conecttec.");

            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Retorno<bool> FinalizarAbastecimentoConecttec(Dominio.ObjetosDeValor.WebService.Abastecimento.FinalizarAbastecimentoConecttec finalizarAbastecimentoConecttec)
        {
            Servicos.Log.TratarErro($"finalizarAbastecimentoConecttec: {Newtonsoft.Json.JsonConvert.SerializeObject(finalizarAbastecimentoConecttec)}");

            try
            {
                if (finalizarAbastecimentoConecttec == null)
                    throw new ServicoException("É obrigatório enviar os objetos da requisição.");

                if (!new Servicos.Embarcador.Integracao.Conecttec.IntegracaoConecttec(_unitOfWork).FinalizarAbastecimento(finalizarAbastecimentoConecttec))
                    throw new ServicoException("Erro ao atualizar status do abastecimento Conecttec.");

            }
            catch (ServicoException excecao)
            {
                _unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }

            return Retorno<bool>.CriarRetornoSucesso(true);
        }

        #endregion
    }
}
