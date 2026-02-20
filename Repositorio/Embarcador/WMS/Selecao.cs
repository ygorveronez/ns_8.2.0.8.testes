using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class Selecao : RepositorioBase<Dominio.Entidades.Embarcador.WMS.Selecao>
    {
        public Selecao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.Selecao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Selecao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.WMS.Selecao> MontarQuery(DateTime dataInicio, DateTime dataFim, int codigoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Selecao>();
            var result = from obj in query select obj;

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.Data >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.Data <= dataFim);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionarios.Any(ped => ped.Usuario.Codigo == codigoFuncionario));

            if ((int)situacao != 0)
                result = result.Where(obj => obj.SituacaoSelecaoSeparacao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.WMS.Selecao> Consultar(DateTime dataInicio, DateTime dataFim, int codigoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = MontarQuery(dataInicio, dataFim, codigoFuncionario, situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(DateTime dataInicio, DateTime dataFim, int codigoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao situacao)
        {
            var result = MontarQuery(dataInicio, dataFim, codigoFuncionario, situacao);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> MontarQueryCarga(int codigoSelecao, DateTime dataInicio, DateTime dataFim, int codigoEmpresa, int codigoFilial, int codigoOrigem, int codigoDestino, double cnpjRemetente, double cnpjDestinatario, string numeroPedidoEmbarcaodor, string numeroCargaEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (codigoSelecao > 0)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SelecaoCarga>();
                var result = from obj in query where obj.Selecao.Codigo == codigoSelecao select obj;

                return result.Select(obj => obj.Carga);
            }
            else
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
                var result = from obj in query select obj;

                if (dataInicio != DateTime.MinValue)
                    result = result.Where(obj => obj.DataCriacaoCarga >= dataInicio);

                if (dataFim != DateTime.MinValue)
                    result = result.Where(obj => obj.DataCriacaoCarga <= dataFim);

                if (codigoEmpresa > 0)
                    result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

                if (codigoFilial > 0)
                    result = result.Where(obj => obj.Filial.Codigo == codigoFilial);

                if (codigoOrigem > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Origem.Codigo == codigoOrigem));

                if (codigoDestino > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destino.Codigo == codigoDestino));

                if (cnpjRemetente > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Remetente.CPF_CNPJ == cnpjRemetente));

                if (cnpjDestinatario > 0)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Destinatario.CPF_CNPJ == cnpjDestinatario));

                if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcaodor))
                {
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcaodor));
                }

                if (!string.IsNullOrWhiteSpace(numeroCargaEmbarcador))
                {
                    result = result.Where(obj => obj.CodigoCargaEmbarcador == numeroCargaEmbarcador || obj.CodigosAgrupados.Contains(numeroCargaEmbarcador));
                }

                result = result.Where(obj => obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica ||
                    obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova ||
                    obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);

                var querySeparacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.SelecaoCarga>();
                var resultSeparacao = from obj in querySeparacao where obj.Selecao.SituacaoSelecaoSeparacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Cancelada select obj;

                result = result.Where(obj => !resultSeparacao.Select(o => o.Carga.Codigo).Contains(obj.Codigo));

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Produtos.Any(pro => pro.Produto != null)));
                else
                    result = result.Where(obj => obj.Pedidos.Any(ped => ped.Pedido.Produtos.Any(pro => (pro.Quantidade > 0 || pro.PesoUnitario > 0) && pro.Produto != null)));

                return result;
            }
        }


        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCarga(int codigoSelecao, DateTime dataInicio, DateTime dataFim, int codigoEmpresa, int codigoFilial, int codigoOrigem, int codigoDestino, double cnpjRemetente, double cnpjDestinatario, string numeroPedidoEmbarcaodor, string numeroCargaEmbarcador, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var result = MontarQueryCarga(codigoSelecao, dataInicio, dataFim, codigoEmpresa, codigoFilial, codigoOrigem, codigoDestino, cnpjRemetente, cnpjDestinatario, numeroPedidoEmbarcaodor, numeroCargaEmbarcador, tipoServicoMultisoftware);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaCarga(int codigoSelecao, DateTime dataInicio, DateTime dataFim, int codigoEmpresa, int codigoFilial, int codigoOrigem, int codigoDestino, double cnpjRemetente, double cnpjDestinatario, string numeroPedidoEmbarcaodor, string numeroCargaEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var result = MontarQueryCarga(codigoSelecao, dataInicio, dataFim, codigoEmpresa, codigoFilial, codigoOrigem, codigoDestino, cnpjRemetente, cnpjDestinatario, numeroPedidoEmbarcaodor, numeroCargaEmbarcador, tipoServicoMultisoftware);

            return result.Count();
        }
    }
}
