using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Servicos.WebServiceCarrefour.CTe
{
    public sealed class CTe
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CTe(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe> BuscarCTes(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, TipoDocumentoRetorno tipoDocumentoRetorno, double remetente, double destinatario, int inicioRegistro, int fimRegistro)
        {
            List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebServiceCarrefour.CTe.CTe>();
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedido(cargaPedido.Codigo, cargaPedido.CargaOrigem.Codigo, remetente, destinatario, true, false, inicioRegistro, fimRegistro);
            Conversores.CTe.CTe servicoConverterCte = new Conversores.CTe.CTe(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                CTesIntegracao.Add(servicoConverterCte.Converter(cargaCTe, tipoDocumentoRetorno));

            return CTesIntegracao;
        }

        public int ContarCTes(int protocoloCarga, int protocoloPedido, double remetente, double destinatario)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiroPorProtocoloCargaEProtocoloPedido(protocoloCarga, protocoloPedido);            

            return repositorioCargaPedidoXMLNotaFiscalCTe.ContarCTesPorCargaPedido(cargaPedido.Codigo, cargaPedido.CargaOrigem.Codigo, remetente, destinatario, semComplementos: true, false);
        }

        #endregion
    }
}
