using System.Collections.Generic;

namespace Servicos.WebService.Carga
{
    public class TipoCarga : ServicoBase
    {        
        public TipoCarga(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador> RetornarTipoCargaEmbarcador(List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador> TiposCargaEmbarcador = new List<Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador>();
            foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga in tiposDeCarga)
            {
                TiposCargaEmbarcador.Add(ConverterObjetoTipoCarga(tipoCarga));
            }
            return TiposCargaEmbarcador;

        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador ConverterObjetoTipoCarga(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga)
        {
            if(tipoCarga != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador tipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador();
                tipoCargaEmbarcador.CNPJsDoTipoCargaNoEmbarcador = new List<string>();


                if (tipoCarga.GrupoPessoas != null)
                {
                    foreach (Dominio.Entidades.Cliente cliente in tipoCarga.GrupoPessoas.Clientes)
                    {
                        tipoCargaEmbarcador.CNPJsDoTipoCargaNoEmbarcador.Add(cliente.CPF_CNPJ_SemFormato);
                    }
                }

                if (tipoCarga.Pessoa != null)
                {
                    if (!tipoCargaEmbarcador.CNPJsDoTipoCargaNoEmbarcador.Contains(tipoCarga.Pessoa.CPF_CNPJ_SemFormato))
                        tipoCargaEmbarcador.CNPJsDoTipoCargaNoEmbarcador.Add(tipoCarga.Pessoa.CPF_CNPJ_SemFormato);
                }


                tipoCargaEmbarcador.CodigoIntegracao = tipoCarga.CodigoTipoCargaEmbarcador;
                tipoCargaEmbarcador.Descricao = tipoCarga.Descricao;
                tipoCargaEmbarcador.ClasseONU = tipoCarga.ClasseONU;
                tipoCargaEmbarcador.CodigoPSNONU = tipoCarga.CodigoPsnONU;
                tipoCargaEmbarcador.ObservacaoONU = tipoCarga.ObservacaoONU;
                tipoCargaEmbarcador.SequenciaONU = tipoCarga.SequenciaONU;

                return tipoCargaEmbarcador;
            }
            else
            {
                return null;
            }
        }

    }
}
