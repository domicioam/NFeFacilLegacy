﻿using NFeFacil.IBGE;
using NFeFacil.ModeloXML.PartesProcesso.PartesNFe.PartesDetalhes.PartesTransporte;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace NFeFacil.ViewModel
{
    public sealed class ICMSTransporteDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICMSTransporte ICMS { get; }

        private Estado _estado;
        public Estado UFEscolhida
        {
            get { return _estado; }
            set
            {
                _estado = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Municipios"));
            }
        }

        public ObservableCollection<Municipio> Municipios
        {
            get
            {
                return UFEscolhida != null ? IBGE.Municipios.Buscar(UFEscolhida).GerarObs() : new ObservableCollection<Municipio>();
            }
        }

        public Municipio MunicipioEscolhido
        {
            get
            {
                return Municipios.Count > 0 ? Municipios.Single(x => x.CodigoMunicípio == long.Parse(ICMS.cMunFG)) : new Municipio();
            }
            set
            {
                ICMS.cMunFG = value.CodigoMunicípio.ToString();
            }
        }

        public ICMSTransporteDataContext(ref ICMSTransporte icms)
        {
            ICMS = icms;
        }
    }
}