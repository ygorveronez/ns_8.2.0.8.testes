using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.WMS
{
    public class Recebimento : RepositorioBase<Dominio.Entidades.Embarcador.WMS.Recebimento>
    {
        public Recebimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.Recebimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Recebimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ContemRecebimentoAbertoMDFe(int codigo, int codigoMDFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Recebimento>();
            var result = from obj in query where (obj.MDFe.Codigo == codigoMDFe || obj.Carga.CargaMDFes.Any(o => o.MDFe.Codigo == codigoMDFe)) && obj.Codigo != codigo && obj.SituacaoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Iniciada select obj;
            return result.Count() > 0;
        }

        public bool ContemRecebimentoAbertoCarga(int codigo, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Recebimento>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.Codigo != codigo && obj.SituacaoRecebimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento.Iniciada select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.WMS.Recebimento> Consulta(int codigoMDFe, string numeroNota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria, int codigoVeiculo, int codigoProdutoEmbarcador, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento situacao, int codigoUsuario, int codigoCarga, string observacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Recebimento>();

            if (data > DateTime.MinValue)
                query = query.Where(obj => obj.Data == data.Date);
            if ((int)situacao >= 0)
                query = query.Where(obj => obj.SituacaoRecebimento == situacao);
            if ((int)tipoRecebimentoMercadoria > 0)
                query = query.Where(obj => obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria);
            if (codigoUsuario > 0)
                query = query.Where(obj => obj.Usuario.Codigo == codigoUsuario);
            if (codigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);
            if (codigoProdutoEmbarcador > 0)
                query = query.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador);
            if (codigoCarga > 0)
                query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            if (codigoMDFe > 0)
                query = query.Where(obj => obj.MDFe.Codigo == codigoMDFe);
            if (!string.IsNullOrWhiteSpace(observacao))
                query = query.Where(obj => obj.Observacao.Contains(observacao));
            if (!string.IsNullOrWhiteSpace(numeroNota))
                query = query.Where(obj => obj.Mercadorias.Any(o => o.Descricao.Contains(numeroNota) || o.NumeroLote.Contains(numeroNota)));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContaConsulta(int codigoMDFe, string numeroNota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimentoMercadoria, int codigoVeiculo, int codigoProdutoEmbarcador, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRecebimento situacao, int codigoUsuario, int codigoCarga, string observacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.Recebimento>();

            if (data > DateTime.MinValue)
                query = query.Where(obj => obj.Data == data.Date);
            if ((int)situacao >= 0)
                query = query.Where(obj => obj.SituacaoRecebimento == situacao);
            if ((int)tipoRecebimentoMercadoria > 0)
                query = query.Where(obj => obj.TipoRecebimentoMercadoria == tipoRecebimentoMercadoria);
            if (codigoUsuario > 0)
                query = query.Where(obj => obj.Usuario.Codigo == codigoUsuario);
            if (codigoVeiculo > 0)
                query = query.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);
            if (codigoProdutoEmbarcador > 0)
                query = query.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador);
            if (codigoCarga > 0)
                query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            if (codigoMDFe > 0)
                query = query.Where(obj => obj.MDFe.Codigo == codigoMDFe);
            if (!string.IsNullOrWhiteSpace(observacao))
                query = query.Where(obj => obj.Observacao.Contains(observacao));
            if (!string.IsNullOrWhiteSpace(numeroNota))
                query = query.Where(obj => obj.Mercadorias.Any(o => o.Descricao.Contains(numeroNota) || o.NumeroLote.Contains(numeroNota)));

            return query.Count();
        }

    }
}
