using Dominio.ObjetosDeValor.WebServiceCarrefour.Carga;
using System;
using System.Threading.Tasks;

namespace SGT.WebServiceCarrefour
{
    public class Cargas : WebServiceBase, ICargas
    {
        #region Métodos Globais 

        public Retorno<Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            ValidarToken();

            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
                Servicos.WebService.Carga.Carga servicoCarga = new Servicos.WebService.Carga.Carga(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado);
                Servicos.WebServiceCarrefour.Conversores.Carga.ProtocolosConverter servicoConverterProtocolos = new Servicos.WebServiceCarrefour.Conversores.Carga.ProtocolosConverter();
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> adicionarPedidoRetorno = servicoCarga.AdicionarPedidoAsync(cargaIntegracao, true, default).GetAwaiter().GetResult();
                Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos protocolos = servicoConverterProtocolos.Converter(adicionarPedidoRetorno.Objeto);
                Retorno<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos> retorno = new Retorno<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos>()
                {
                    CodigoMensagem = adicionarPedidoRetorno.CodigoMensagem,
                    DataRetorno = adicionarPedidoRetorno.DataRetorno,
                    Mensagem = adicionarPedidoRetorno.Mensagem,
                    Objeto = protocolos,
                    Status = adicionarPedidoRetorno.Status
                };

                return retorno;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.WebServiceCarrefour.Carga.Protocolos>.CriarRetornoExcecao("Ocorreu uma falha ao processar a requisição.");
            }
        }

        #endregion Métodos Globais

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;
        }

        #endregion Métodos Privado
    }
}
