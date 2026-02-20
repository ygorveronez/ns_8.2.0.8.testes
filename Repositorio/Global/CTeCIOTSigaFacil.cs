using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class CTeCIOTSigaFacil : RepositorioBase<Dominio.Entidades.CTeCIOTSigaFacil>, Dominio.Interfaces.Repositorios.CTeCIOTSigaFacil
    {
        public CTeCIOTSigaFacil(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CTeCIOTSigaFacil BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeCIOTSigaFacil>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CTeCIOTSigaFacil> BuscarPorCIOT(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeCIOTSigaFacil>();

            var result = from obj in query where obj.CIOT.Codigo == codigoCIOT select obj;

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.DocumentoContratoTransporteRodoviario> RelatorioContratoTransporte(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeCIOTSigaFacil>();

            var result = from obj in query where obj.CIOT.Codigo == codigoCIOT select obj;

            return result.OrderBy(o => o.CTe.Numero).Select(o => new Dominio.ObjetosDeValor.Relatorios.DocumentoContratoTransporteRodoviario()
            {
                EspecieMercadoria = o.EspecieMercadoria,
                Mercadoria = o.CTe.ProdutoPredominanteCTe,
                NumeroCTe = o.CTe.Numero,
                NumeroNotaFiscal = o.NumeroNotaFiscal,
                PesoBruto = o.PesoBruto,
                PesoLotacao = o.PesoLotacao,
                QuantidadeMercadoria = o.QuantidadeMercadoria,
                SerieCTe = o.CTe.Serie.Numero,
                ValorAdiantamento = o.ValorAdiantamento,
                ValorCartaoPedagio = o.ValorCartaoPedagio,
                ValorFrete = o.ValorFrete,
                ValorINSS = o.ValorINSS,
                ValorIRRF = o.ValorIRRF,
                ValorMercadoriaKG = o.ValorMercadoriaKG,
                ValorOutrosDescontos = o.ValorOutrosDescontos,
                ValorPedagio = o.ValorPedagio,
                ValorSeguro = o.ValorSeguro,
                ValorSENAT = o.ValorSENAT,
                ValorSEST = o.ValorSEST,
                ValorTarifaEmissaoCartao = o.ValorTarifaEmissaoCartao,
                ValorTarifaFrete = o.ValorTarifaFrete,
                ValorTotalMercadoria = o.ValorTotalMercadoria,
                ValorAbastecimento = o.ValorAbastecimento != null ? o.ValorAbastecimento : 0
            }).ToList();
        }


        public List<Dominio.Entidades.CTeCIOTSigaFacil> BuscarCTesPorCIOT(int codigoCIOT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeCIOTSigaFacil>();

            var result = from obj in query where obj.CIOT.Codigo == codigoCIOT select obj;

            return result.ToList();
        }

        public Dominio.Entidades.CTeCIOTSigaFacil BuscarCTePorCIOT(int codigoCIOT, int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeCIOTSigaFacil>();

            var result = from obj in query where obj.CIOT.Codigo == codigoCIOT && obj.CTe.Codigo == codigoCTe select obj;

            return result.FirstOrDefault();
        }
    }
}
