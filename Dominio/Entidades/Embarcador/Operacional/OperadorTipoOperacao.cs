//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Dominio.Entidades.Embarcador.Operacional
//{
//    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OPERADOR_TIPO_OPERACAO", EntityName = "OperadorTipoOperacao", Name = "Dominio.Entidades.Embarcador.Operacional.OperadorTipoOperacao", NameType = typeof(OperadorTipoOperacao))]
//    public class OperadorTipoOperacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Operacional.OperadorTipoOperacao>
//    {
//        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OTO_CODIGO")]
//        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
//        public virtual int Codigo { get; set; }

//        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OperadorLogistica", Column = "OPL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
//        public virtual Dominio.Entidades.Embarcador.Operacional.OperadorLogistica OperadorLogistica { get; set; }

//        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoOperacaoEmissao", Column = "FTO_TIPO_OPERACAO_EMISSAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao), NotNull = true)]
//        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao TipoOperacaoEmissao { get; set; }

//        public virtual bool Equals(OperadorTipoOperacao other)
//        {
//            if (other.Codigo == this.Codigo)
//                return true;
//            else
//                return false;
//        }

//    }
//}
