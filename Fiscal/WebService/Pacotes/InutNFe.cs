﻿using BaseGeral;
using Fiscal.Certificacao;
using BaseGeral.ModeloXML.PartesAssinatura;
using Fiscal.WebService.Pacotes.PartesInutNFe;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Fiscal.WebService.Pacotes
{
    [XmlRoot("inutNFe", Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public sealed class InutNFe : ISignature
    {
        [XmlAttribute("versao")]
        public string Versao { get; set; }

        [XmlElement("infInut", Order = 0)]
        public InfInut Info { get; set; }

        [XmlElement("Signature", Order = 1, Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Assinatura Signature { get; set; }

        public InutNFe() { }
        public InutNFe(InfInut info)
        {
            Versao = "4.00";
            Info = info;
            Signature = null;
        }

        public async Task<(bool, string)> PrepararEvento(AssinaFacil assinador, X509Certificate2 cert)
        {
            assinador.Nota = this;
            return await assinador.Assinar<InutNFe>(cert, Info.Id, "infInut");
        }
    }
}
