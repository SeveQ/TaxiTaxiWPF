﻿#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using SeveQsDataBase;

#endregion

namespace TaxiTaxiWPF.TaxiData
{
    public class Fahrzeug : DataBase
    {
        private float _besetztEnde;
        private float _besetztAnfang;
        private int _nummer;
        private float _preisEnde;
        private float _preisAnfang;
        private float _totalEnde;
        private float _totalAnfang;
        private int _tourenEnde;
        private int _tourenAnfang;

        public Fahrzeug()
        {
            Fahrten = new IndexedObservableCollection<Fahrt>();
        }

        [ObserveProperty]
        [ObserveProperty(Dependency = "Fahrt.Index")]
        public ICollectionView FahrtenView
        {
            get
            {
                if (View == null) View = CollectionViewSource.GetDefaultView(_fahrten);
                var lcv = (ListCollectionView)View;
                if (lcv.IsAddingNew) lcv.CommitNew();

                View.SortDescriptions.Clear();
                View.SortDescriptions.Add(new SortDescription("Index", ListSortDirection.Ascending));
                return View;
            }
        }

        private IndexedObservableCollection<Fahrt> _fahrten;
        public IndexedObservableCollection<Fahrt> Fahrten
        {
            get { return _fahrten; }
            set
            {
                _fahrten = value;
                foreach (var item in value)
                {
                    item.Parent = this;
                    item.IndexGroup = _fahrten;
                }

                _fahrten.CollectionChanged += (x, y) =>
                    {
                        lock (_fahrten)
                        {
                            foreach (IHasParent z in y.NewItems ?? new List<IHasParent>())
                            {
                                z.Parent = this;
                            }

                            foreach (IIndexed z in y.NewItems ?? new List<IIndexed>())
                            {
                                z.Index = _fahrten.Max(p => p.Index) + 1;
                                z.IndexGroup = _fahrten;
                            }
                        }
                        OnPropertyChanged("Fahrten");
                    };

                OnPropertyChanged();
            }
        }

        //[ObserveProperty(Dependency = "Nummer")]
        public int Nummer
        {
            get { return _nummer; }
            set
            {
                _nummer = value;
                OnPropertyChanged();
            }
        }

        //[ObserveProperty(Dependency = "TotalAnfang")]
        public float TotalAnfang
        {
            get { return _totalAnfang; }
            set
            {
                _totalAnfang = value;
                OnPropertyChanged();
            }
        }

        //[ObserveProperty(Dependency = "TotalEnde")]
        public float TotalEnde
        {
            get { return _totalEnde; }
            set
            {
                _totalEnde = value;
                OnPropertyChanged();
            }
        }

        [ObserveProperty(Dependency = "TotalAnfang")]
        [ObserveProperty(Dependency = "TotalEnde")]
        public float TotalTotal
        {
            get { return (float)Math.Round(TotalEnde - TotalAnfang, 1); }
        }

        //[ObserveProperty(Dependency = "BesetztAnfang")]
        public float BesetztAnfang
        {
            get { return _besetztAnfang; }
            set
            {
                _besetztAnfang = value;
                OnPropertyChanged();
            }
        }

        //[ObserveProperty(Dependency = "BesetztEnde")]
        public float BesetztEnde
        {
            get { return _besetztEnde; }
            set
            {
                _besetztEnde = value;
                OnPropertyChanged();
            }
        }

        [ObserveProperty(Dependency = "BesetztAnfang")]
        [ObserveProperty(Dependency = "BesetztEnde")]
        public float BesetztTotal
        {
            get { return (float)Math.Round(BesetztEnde - BesetztAnfang, 1); }
        }

        [ObserveProperty(Dependency = "BesetztTotal")]
        public float Besetzt2x
        {
            get { return (float)Math.Round(BesetztTotal * 2.0F, 1); }
        }

        [ObserveProperty(Dependency = "TotalTotal")]
        [ObserveProperty(Dependency = "Besetzt2x")]
        [ObserveProperty(Dependency = "LeerAngegebenSumme")]
        public float Leer
        {
            get { return (float)Math.Round(TotalTotal - Besetzt2x - LeerAngegebenSumme, 1); }
        }

        [ObserveProperty(Dependency = "Fahrt.KM")]
        public float LeerAngegebenSumme
        {
            get { return Fahrten.Sum(p => p.KM ?? 0.0F); }
        }

        //[ObserveProperty(Dependency = "TourenAnfang")]
        public int TourenAnfang
        {
            get { return _tourenAnfang; }
            set
            {
                _tourenAnfang = value;
                OnPropertyChanged();
            }
        }

        //[ObserveProperty(Dependency = "TourenEnde")]
        public int TourenEnde
        {
            get { return _tourenEnde; }
            set
            {
                _tourenEnde = value;
                OnPropertyChanged();
            }
        }

        [ObserveProperty(Dependency = "TourenAnfang")]
        [ObserveProperty(Dependency = "TourenEnde")]
        public int TourenTotal
        {
            get { return TourenEnde - TourenAnfang; }
        }

        //[ObserveProperty(Dependency = "PreisAnfang")]
        public float PreisAnfang
        {
            get { return _preisAnfang; }
            set
            {
                _preisAnfang = value;
                OnPropertyChanged();
            }
        }

        //[ObserveProperty(Dependency = "PreisEnde")]
        public float PreisEnde
        {
            get { return _preisEnde; }
            set
            {
                _preisEnde = value;
                OnPropertyChanged();
            }
        }

        #region Calculated Properties

        [ObserveProperty(Dependency = "Fahrt.Preis")]
        public float TourPricesSum
        {
            get { return Fahrten.Sum(p => p.Preis ?? 0.0F); }
        }

        [ObserveProperty(Dependency = "PreisAnfang")]
        [ObserveProperty(Dependency = "PreisEnde")]
        public float PreisTotal
        {
            get { return (float)Math.Round(PreisEnde - PreisAnfang, 2); }
        }

        [ObserveProperty(Dependency = "Fahrt.APES")]
        public float SummeOhneUhr
        {
            get { return Fahrten.Sum(p => p.APES ?? 0.0F); }
        }

        [ObserveProperty(Dependency = "PreisTotal")]
        [ObserveProperty(Dependency = "SummeOhneUhr")]
        public float PreisInklAPES
        {
            get { return PreisTotal + SummeOhneUhr; }
        }

        [ObserveProperty(Dependency = "PreisInklAPES")]
        [ObserveProperty(Dependency = "TourPricesSum")]
        [ObserveProperty(Dependency = "SummeOhneUhr")]
        public float Preisdifferenz
        {
            get { return (float)Math.Round(PreisInklAPES - (TourPricesSum + SummeOhneUhr), 2); }
        }

        [ObserveProperty(Dependency = "Fahrt.Sonderausgabe")]
        [ObserveProperty(Dependency = "Fahrt.KM")]
        [ObserveProperty(Dependency = "Fahrt.Preis")]
        [ObserveProperty(Dependency = "Fahrt.APES")]
        public int TourCount
        {
            get { return Fahrten.Count(p => p.Preis != null); }
        }

        [ObserveProperty(Dependency = "Fahrt.Bezahlt")]
        public float BargeldErhaltenOhneTip
        {
            get
            {
                // lock (Fahrten)
                //{
                    float sum = Fahrten.Sum(p => p.Bezahlt);
                    return sum;
                //}
            }
        }

        #endregion

        public override string Name
        {
            get { return String.Format("Fahrzeug Nr. {0}", Nummer); }
        }

        [ObserveProperty(Dependency = "Fahrt.Preis")]
        [ObserveProperty(Dependency = "Fahrt.Rechnungsfahrt")]
        public float AbzugRechnungsfahrten
        {
            get { return Fahrten.Where(f => f.Rechnungsfahrt).Sum(f => f.Preis ?? 0.0F); }
        }

        public void ReindexTours()
        {
            int uoIdx = 1;
            foreach (var item in Fahrten.OrderBy(p => p.Index).ThenBy(p => p.SortOrder))
            {
                if (item.Index == 0)
                {
                    while (Fahrten.Any(p => p.Index != 0 && p.Index == uoIdx)) uoIdx++;
                    item.Index = uoIdx;
                }
            }
        }
    }
}