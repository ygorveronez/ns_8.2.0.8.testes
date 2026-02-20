using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace Servicos.Embarcador.Logistica
{
    public class DiariaAutomaticaCalculoFrete
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public DiariaAutomaticaCalculoFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Calcula o valor da Diária Automática, passando a diária automática.
        /// </summary>
        /// <param name="parametrosCalcularValorDiariaAutomatica"></param>
        /// <param name="configuracaoEmbarcador"></param>
        /// <param name="tipoServicoMultisoftware"></param>
        /// <returns></returns>
        public Dominio.ObjetosDeValor.Embarcador.Logistica.CalculoFreteDiariaAutomatica CalcularValorDiariaAutomatica(Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, TipoServicoMultisoftware tipoServicoMultisoftware)
        {

            return CalcularValorDiariaAutomatica(new Dominio.ObjetosDeValor.Embarcador.Logistica.ParametroCalcularValorDiariaAutomatica
            {
                CodigoCarga = diariaAutomatica.Carga.Codigo,
                DataInicio = diariaAutomatica.DataInicioCobranca,
                Minutos = diariaAutomatica.TempoTotal,
                LocalFreeTime = diariaAutomatica.LocalFreeTime,
            }, configuracaoEmbarcador, TipoServicoMultisoftware.MultiEmbarcador);
        }

        /// <summary>
        /// Calcula o valor da Diária Automática, passando os parâmetros necessários.
        /// </summary>
        /// <param name="parametrosCalcularValorDiariaAutomatica"></param>
        /// <param name="configuracaoEmbarcador"></param>
        /// <param name="tipoServicoMultisoftware"></param>
        /// <returns></returns>
        public Dominio.ObjetosDeValor.Embarcador.Logistica.CalculoFreteDiariaAutomatica CalcularValorDiariaAutomatica(Dominio.ObjetosDeValor.Embarcador.Logistica.ParametroCalcularValorDiariaAutomatica parametrosCalcularValorDiariaAutomatica, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(parametrosCalcularValorDiariaAutomatica.CodigoCarga);

            return ObterCalculoFreteOcorrenciaPorTabelaFrete(parametrosCalcularValorDiariaAutomatica, carga, configuracaoEmbarcador, tipoServicoMultisoftware);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.CalculoFreteDiariaAutomatica ObterCalculoFreteOcorrenciaPorTabelaFrete(Dominio.ObjetosDeValor.Embarcador.Logistica.ParametroCalcularValorDiariaAutomatica parametrosCalcularValorOcorrencia, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo); ;

            Servicos.Embarcador.Carga.Frete serFrete = new Servicos.Embarcador.Carga.Frete(_unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(_unitOfWork);

            StringBuilder mensagemRetorno = new StringBuilder();
            List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = serFrete.ObterTabelasFrete(carga, _unitOfWork, tipoServicoMultisoftware, configuracao, ref mensagemRetorno, false, null, false, false, localFreeTime: parametrosCalcularValorOcorrencia.LocalFreeTime);

            if (mensagemRetorno.Length > 0)
                throw new ServicoException(mensagemRetorno.ToString());

            if (tabelasFrete.Count == 0)
                throw new ServicoException("Não foi localizada uma tabela de frete compatível com as informações da diária automática.");

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = tabelasFrete[0];
            Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = serFrete.ObterParametrosCalculoFretePorCarga(tabelaFrete, carga, cargaPedidos, false, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, configuracao);

            if (parametrosCalculoFrete == null)
                throw new ServicoException("Não foi possível obter os parametros para cálculo de frete da carga pois os pedidos da carga não são cálculaveis (exemplo, somente pedidos de pallet)");

            parametrosCalculoFrete.DataInicialViagem = parametrosCalcularValorOcorrencia.DataInicio;
            parametrosCalculoFrete.DataFinalViagem = parametrosCalcularValorOcorrencia.DataInicio.AddMinutes(parametrosCalcularValorOcorrencia.Minutos);
            parametrosCalculoFrete.Minutos = parametrosCalcularValorOcorrencia.Minutos;

            Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculoFrete, tabelaFrete, tipoServicoMultisoftware).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete dadosCalculoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.DadosCalculoFrete();

            if (tabelaFreteCliente == null)
            {
                dadosCalculoFrete.FreteCalculado = false;
                dadosCalculoFrete.FreteCalculadoComProblemas = true;
                dadosCalculoFrete.MensagemRetorno = mensagemRetorno.ToString();
            }
            else
            {
                if (tabelaFreteCliente.TabelaFrete.ParametroBase.HasValue)
                    svcFreteCliente.SetarValoresTabelaFreteComParametroBase(ref dadosCalculoFrete, parametrosCalculoFrete, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);
                else
                    svcFreteCliente.SetarValoresTabelaFreteSemParametroBase(ref dadosCalculoFrete, parametrosCalculoFrete, tabelaFreteCliente, tipoServicoMultisoftware, configuracao);

                dadosCalculoFrete.FreteCalculado = true;
            }

            decimal valorTotalFrete = dadosCalculoFrete.ValorFrete;

            if (dadosCalculoFrete.Componentes != null)
                valorTotalFrete += (from obj in dadosCalculoFrete.Componentes where obj.SomarComponenteFreteLiquido || obj.DescontarComponenteFreteLiquido select obj.DescontarComponenteFreteLiquido ? obj.ValorComponente * -1 : obj.ValorComponente).Sum();

            if (!dadosCalculoFrete.FreteCalculado)
                throw new ServicoException(dadosCalculoFrete.MensagemRetorno);

            return new Dominio.ObjetosDeValor.Embarcador.Logistica.CalculoFreteDiariaAutomatica()
            {
                ValorCalculadoPorTabelaFrete = true,
                ValorDiariaAutomatica = valorTotalFrete,
                ListaComposicaoFrete = dadosCalculoFrete.ComposicaoFrete,
            };
        }

        #endregion
    }
}
