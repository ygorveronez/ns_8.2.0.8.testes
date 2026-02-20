using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class DocumentoFaturamento
    {
        #region Métodos Públicos

        public static void RemoverDocumentoProvisao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            RemoverDocumentoProvisao(null, carga, unitOfWork);
        }

        public static void RemoverDocumentoProvisao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            RemoverDocumentoProvisao(cte, null, unitOfWork);
        }

        public static void AdicionarDocumentoProvisao(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamentoProvisao = documentoFaturamento.Clonar();

            Utilidades.Object.DefinirListasGenericasComoNulas(documentoFaturamentoProvisao);

            documentoFaturamentoProvisao.ValorAFaturar = documentoFaturamentoProvisao.ValorEmFatura;
            documentoFaturamentoProvisao.TipoLiquidacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.PagamentoTransportador;
            documentoFaturamentoProvisao.Veiculos = documentoFaturamento.Veiculos.ToList();
            documentoFaturamentoProvisao.Motoristas = documentoFaturamento.Motoristas.ToList();
            documentoFaturamentoProvisao.NumeroPedidoCliente = documentoFaturamento.NumeroPedidoCliente.ToList();
            documentoFaturamentoProvisao.NumeroPedidoOcorrenciaCliente = documentoFaturamento.NumeroPedidoOcorrenciaCliente.ToList();

            repDocumentoFaturamento.Inserir(documentoFaturamentoProvisao);
        }

        public static void CancelarDocumentoFaturamentoPorCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoFaturamentos = repDocumentoFaturamento.BuscarDocumentoAtivoPorCTe(cte.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentoFaturamentos)
            {
                if (cte.Status == "C")
                {
                    documentoFaturamento.DataCancelamento = cte.DataCancelamento;
                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Cancelado;
                }
                else if (cte.Status == "Z")
                {
                    documentoFaturamento.DataAnulacao = cte.DataAnulacao;
                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Anulado;
                }

                repDocumentoFaturamento.Atualizar(documentoFaturamento);
            }
        }

        #endregion

        #region Métodos Privados

        private static void RemoverDocumentoProvisao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = null;

            if (cte != null)
                documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(cte.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.PagamentoTransportador);
            else if (carga != null)
                documentoFaturamento = repDocumentoFaturamento.BuscarPorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLiquidacao.PagamentoTransportador);

            if (documentoFaturamento != null)
            {
                documentoFaturamento.NumeroPedidoCliente = null;
                documentoFaturamento.NumeroPedidoOcorrenciaCliente = null;
                documentoFaturamento.Veiculos = null;
                documentoFaturamento.Motoristas = null;

                repDocumentoFaturamento.Deletar(documentoFaturamento);
            }
        }

        #endregion
    }
}
