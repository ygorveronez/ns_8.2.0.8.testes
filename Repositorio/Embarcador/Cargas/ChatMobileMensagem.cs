using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class ChatMobileMensagem : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>
    {
        public ChatMobileMensagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut
                .Fetch(obj => obj.Remetente)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return resut
                .Fetch(obj => obj.Remetente)
                .OrderBy(obj => obj.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>();
            var resut = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;
            return resut
                .Fetch(obj => obj.Remetente)
                .OrderBy(obj => obj.Codigo)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem BuscarUltimoPorCarga(int codigoCarga, int remetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Remetente.Codigo != remetente select obj;
            return resut
                .OrderByDescending(obj => obj.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem BuscarUltimoPorPedido(int codigoPedido, int remetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>();
            var resut = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Remetente.Codigo != remetente select obj;
            return resut
                .OrderByDescending(obj => obj.Codigo)
                .FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem> BuscarNaoLidaPorCargas(List<int> codigosCarga, int rementente)
        {

            var queryGrupo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo) && o.Remetente.Codigo != rementente && o.MensagemLida == false)
                .GroupBy(o => o.Carga.Codigo)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem { CodigoCarga = o.Max(e => e.Carga.Codigo), CodigoMensagem = o.Key });


            return queryGrupo.ToList();
        }

        public bool ExisteChatPorPedidoRemetente(int codigoPedido, int rementente)
        {

            var queryGrupo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>()
                .Any(o => o.Pedido.Codigo == codigoPedido && o.Remetente.Codigo == rementente);

            return queryGrupo;
        }

        public void MarcarTodasComoLidasPorRemetente(int codigoCarga, int remetente, DateTime dataConfirmacaoLeitura)
        {
            UnitOfWork.Sessao.CreateQuery("update ChatMobileMensagem set DataConfirmacaoLeitura = :DataConfirmacaoLeitura, MensagemLida = :MensagemLida where Carga.Codigo = :codigoCarga and Remetente.Codigo <> :Remetente")
                .SetInt32("codigoCarga", codigoCarga)
                .SetInt32("Remetente", remetente)
                .SetDateTime("DataConfirmacaoLeitura", dataConfirmacaoLeitura)
                .SetBoolean("MensagemLida", true)
                .ExecuteUpdate();
        }

        public void MarcarTodasComoLidasPorRemetentePedido(int codigoPedido, int remetente, DateTime dataConfirmacaoLeitura)
        {
            UnitOfWork.Sessao.CreateQuery("update ChatMobileMensagem set DataConfirmacaoLeitura = :DataConfirmacaoLeitura, MensagemLida = :MensagemLida where Pedido.Codigo = :codigoPedido and Remetente.Codigo <> :Remetente")
                .SetInt32("codigoPedido", codigoPedido)
                .SetInt32("Remetente", remetente)
                .SetDateTime("DataConfirmacaoLeitura", dataConfirmacaoLeitura)
                .SetBoolean("MensagemLida", true)
                .ExecuteUpdate();
        }


        //public void MarcarLida(int codigoCarga, DateTime data)
        //{
        //    try
        //    {

        //        if (UnitOfWork.IsActiveTransaction())
        //        {
        //            UnitOfWork.Sessao.CreateQuery("UPDATE ChatMobileMensagem c set MensagemLida = 1  WHERE c.Carga.Codigo = :codigoCarga and c.DataCriacao < :data ").SetInt32("codigoCarga", codigoCarga).SetDateTime("data", data).ExecuteUpdate();
        //        }
        //        else
        //        {
        //            using (UnitOfWork.Start())
        //            {
        //                try
        //                {
        //                    UnitOfWork.Sessao.CreateQuery("UPDATE ChatMobileMensagem c set MensagemLida = 1  WHERE c.Carga.Codigo = :codigoCarga and c.DataCriacao < :data ").SetInt32("codigoCarga", codigoCarga).SetDateTime("data", data).ExecuteUpdate();

        //                    UnitOfWork.Sessao.Transaction.Commit();
        //                }
        //                catch
        //                {
        //                    UnitOfWork.Rollback();
        //                    throw;
        //                }
        //            }
        //        }
        //    }
        //    catch (NHibernate.Exceptions.GenericADOException ex)
        //    {
        //        if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
        //        {
        //            System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
        //            if (excecao.Number == 547)
        //            {
        //                throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
        //            }
        //        }
        //        throw;
        //    }
        //}


        public Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem BuscarNaoLidaPorCarga(int codigoCarga, int remetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>()
                .Where(o => codigoCarga == o.Carga.Codigo && o.MensagemLida == false && o.Remetente.Codigo != remetente)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem { CodigoCarga = o.Carga.Codigo, CodigoMensagem = o.Codigo });

            return query.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> BuscarNaoLidaOuLidaPorCargaERemetente(int codigoCarga, int remetente, bool lida)
        {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem>();
                var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Remetente.Codigo != remetente && obj.MensagemLida == lida  select obj;
                return resut
                    .OrderByDescending(obj => obj.Codigo)
                    .ToList();
        }


    }
}
