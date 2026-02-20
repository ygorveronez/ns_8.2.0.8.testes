using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System.Linq;
using Repositorio;
using Repositorio.Embarcador.Cargas;
using Repositorio.Embarcador.Pedidos;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public class AssociacaoBalsa
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion 

        #region Construtores

        public AssociacaoBalsa(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion 

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga ProcessarAssociacaoBalsa(Dominio.Entidades.Embarcador.Logistica.AssociacaoBalsa associacaoBalsa)
        {
            Repositorio.Embarcador.Cargas.Carga repsitorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga retorno = new Dominio.ObjetosDeValor.Embarcador.Pedido.RetornoImportacaoPedidoGerarCarga();

            try
            {
                _unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = associacaoBalsa.Carga;

                if (carga == null)
                    throw new ServicoException("Carga não foi localizada.");

                if (associacaoBalsa.Balsa == null)
                    throw new ServicoException("Balsa não foi localizada.");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repositorioCargaCTe.BuscarCTePorCarga(carga.Codigo);

                carga.Balsa = associacaoBalsa.Balsa;

                _auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
                _auditado.Usuario = associacaoBalsa.Usuario;
                _auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario;

                if (pedidos != null && pedidos.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    {
                        pedido.Balsa = associacaoBalsa.Balsa;

                        repositorioPedido.Atualizar(pedido);
                        Servicos.Auditoria.Auditoria.Auditar(_auditado, pedido, $"Adicionou uma balsa ({pedido.Balsa.Descricao}) na carga.", _unitOfWork);
                    }
                }

                if (ctes != null & ctes.Count > 0)
                {
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                    {
                        cte.Balsa = associacaoBalsa.Balsa;

                        repositorioCTe.Atualizar(cte);
                    }
                }

                repsitorioCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Adicionou uma balsa ({carga.Balsa.Descricao}) na carga.", _unitOfWork);

                _unitOfWork.CommitChanges();

                retorno.Sucesso = true;

                return retorno;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);

                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
                return retorno;
            }
        }

        #endregion
    }
}