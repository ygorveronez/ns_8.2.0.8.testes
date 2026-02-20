using Repositorio;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class Seguro : ServicoBase
    {        

        public Seguro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ConverterSegurosCTeParaSeguro(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro> segurosCTe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            if (segurosCTe != null)
            {
                foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiroSeguro seguroCTe in segurosCTe)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();
                    seguro.Apolice = seguro.Apolice;
                    seguro.ResponsavelSeguro = seguro.ResponsavelSeguro;
                    seguro.Seguradora = seguro.Seguradora;
                    seguro.CNPJSeguradora = seguro.CNPJSeguradora;
                    seguro.Averbacao = seguro.Averbacao;
                    seguro.Valor = seguro.Valor;
                    seguros.Add(seguro);
                }
            }

            return seguros;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ConverterSegurosCTeParaSeguro(List<Dominio.Entidades.SeguroCTE> segurosCTe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            if (segurosCTe != null)
            {
                foreach (Dominio.Entidades.SeguroCTE seguroCTe in segurosCTe)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();
                    seguro.Apolice = seguro.Apolice;
                    seguro.ResponsavelSeguro = seguro.ResponsavelSeguro;
                    seguro.Seguradora = seguro.Seguradora;
                    seguro.CNPJSeguradora = seguro.CNPJSeguradora;
                    seguro.Averbacao = seguro.Averbacao;
                    seguro.Valor = seguro.Valor;
                    seguros.Add(seguro);
                }
            }

            return seguros;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> ConverterDynamicParaSeguro(dynamic dynSeguros, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros = new List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro>();

            if (dynSeguros != null)
            {
                foreach (var dynSeguro in dynSeguros)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguro = new Dominio.ObjetosDeValor.Embarcador.CTe.Seguro();
                    seguro.ResponsavelSeguro = (Dominio.Enumeradores.TipoSeguro)dynSeguro.CodigoResponsavel;
                    seguro.Seguradora = (string)dynSeguro.Seguradora;
                    seguro.CNPJSeguradora = Utilidades.String.OnlyNumbers((string)dynSeguro.CNPJSeguradora);
                    seguro.Apolice = (string)dynSeguro.NumeroApolice;
                    seguro.Averbacao = (string)dynSeguro.NumeroAverbacao;

                    decimal valorSeguro;
                    decimal.TryParse(dynSeguro.Valor.ToString(), out valorSeguro);
                    seguro.Valor = valorSeguro;
                    seguros.Add(seguro);
                }
            }

            return seguros;
        }

        public void SalvarSeguros(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.SeguroCTE repSeguro = new Repositorio.SeguroCTE(unitOfWork);

            if (cte.Codigo > 0)
                repSeguro.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguroIntegracao in seguros)
            {
                Dominio.Entidades.SeguroCTE seguro = new Dominio.Entidades.SeguroCTE();
                seguro.CTE = cte;
                seguro.NomeSeguradora = Utilidades.String.Left(seguroIntegracao.Seguradora, 30);
                seguro.CNPJSeguradora = seguroIntegracao.CNPJSeguradora;
                seguro.NumeroApolice = seguroIntegracao.Apolice;
                seguro.NumeroAverbacao = seguroIntegracao.Averbacao;
                seguro.Tipo = seguroIntegracao.ResponsavelSeguro;
                seguro.Valor = seguroIntegracao.Valor;

                repSeguro.Inserir(seguro);
            }
        }

        public void SalvarSegurosPreCte(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, List<Dominio.ObjetosDeValor.Embarcador.CTe.Seguro> seguros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.SeguroPreCTE repSeguroPreCte = new Repositorio.SeguroPreCTE(unitOfWork);

            if (preCte.Codigo > 0)
                repSeguroPreCte.DeletarPorPreCTe(preCte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.Seguro seguroIntegracao in seguros)
            {
                Dominio.Entidades.SeguroPreCTE seguro = new Dominio.Entidades.SeguroPreCTE();
                seguro.PreCTE = preCte;
                seguro.NomeSeguradora = Utilidades.String.Left(seguroIntegracao.Seguradora, 30);
                seguro.NumeroApolice = seguroIntegracao.Apolice;
                seguro.NumeroAverbacao = seguroIntegracao.Apolice;
                seguro.Tipo = seguroIntegracao.ResponsavelSeguro;
                seguro.Valor = seguroIntegracao.Valor;

                repSeguroPreCte.Inserir(seguro);
            }
        }
    }
}
