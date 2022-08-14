using System;
using System . Collections;
using System . Collections . Generic;
using System . ComponentModel;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace  NewWpfDev. ViewModels
{
      public class nworder : IComparable
      {
            #region PropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged ( string propertyName )
            {
                  //PropertyChanged?.Invoke ( this, new PropertyChangedEventArgs ( propertyName ) );
                  //			this . VerifyPropertyName ( propertyName );

                  if ( this . PropertyChanged != null )
                  {
                        var e = new PropertyChangedEventArgs ( propertyName );
                        this . PropertyChanged ( this , e );
                  }
            }

            #endregion PropertyChanged

            #region IComparable implementation
            //implement IComparable for the class = (Calls other comparer classes )
            int IComparable.CompareTo ( object obj )
            {
                  nworder nwo1 = ( nworder ) obj;
                  return String . Compare ( this . OrderId . ToString ( ) , nwo1 . OrderId . ToString ( ) );
            }

            public static IComparer SortOrderIdAscending ( )
            {
                  return ( IComparer ) new SortOrderIdsAscending ( );
            }
            public static IComparer SortOrderIddescending ( )
            {
                  return ( IComparer ) new SortOrderIdsdescending ( );
            }
            #endregion

            #region IComparer classes

            public class SortOrderIdsAscending : IComparer<nworder>
            {

                  public int Compare ( nworder x , nworder y )
                  {
                        if ( x . OrderId > y . OrderId )
                              return 1;
                        else if ( x . OrderId < y . OrderId )
                              return -1;
                        else
                              return 0;
                  }
            }
            public class SortOrderIdsdescending : IComparer<nworder>
            {
                  public int Compare ( nworder x , nworder y )
                  {
                        if ( x . OrderId < y . OrderId )
                              return 1;
                        else if ( x . OrderId > y . OrderId )
                              return -1;
                        else
                              return 0;
                  }
            }
            #endregion

            /// <summary>
            /// Returns whether an exception is thrown, or if a Debug.Fail() is used
            /// when an invalid property name is passed to the VerifyPropertyName method.
            /// The default value is false, but subclasses used by unit tests might
            /// override this property's getter to return true.
            /// </summary>
            //protected virtual bool ThrowOnInvalidPropertyName
            //{
            //      get; private set;
            //}

            #region declarations

            //private int identity;
            private int orderId;

            private string customerId;
            private int employeeId;
            private DateTime dateTime;
            private DateTime requiredDate;
            private DateTime shippedDate;
            private int shipVia;
            private decimal freight;
            private string shipName;
            private string shipAddress;
            private string shipCity;
            private string shipRegion;
            private string shipPostalCode;
            private string shipCountry;
            private nworder selectedItem;
            private int selectedIndex;
            private int orderDetailsTotal;
            public int OrderDetailsTotal
            {
                  get
                  {
                        return orderDetailsTotal;
                  }
                  set
                  {
                        orderDetailsTotal = value;
                        OnPropertyChanged ( "OrderDetailsTotal" );
                  }
            }
            public int SelectedIndex
            {
                  get
                  {
                        return selectedIndex;
                  }
                  set
                  {
                        selectedIndex = value;
                  }
            }

            public nworder SelectedItem
            {
                  get
                  {
                        return selectedItem;
                  }
                  set
                  {
                        selectedItem = value;
                        OnPropertyChanged ( "SelectedItem" );
                  }
            }
            public int OrderId
            {
                  get
                  {
                        return orderId;
                  }
                  set
                  {
                        orderId = value;
                        OnPropertyChanged ( "OrderId" );
                  }
            }

            public string CustomerId
            {
                  get
                  {
                        return customerId;
                  }
                  set
                  {
                        customerId = value;
                        OnPropertyChanged ( "CustomerId" );
                  }
            }

            public int EmployeeId
            {
                  get
                  {
                        return employeeId;
                  }
                  set
                  {
                        employeeId = value;
                        OnPropertyChanged ( "EmployeeId" );
                  }
            }


            public DateTime OrderDate
            {
                  get
                  {
                        return dateTime;
                  }
                  set
                  {
                        dateTime = value;
                        OnPropertyChanged ( "OrderDate" );
                  }
            }

            public DateTime RequiredDate
            {
                  get
                  {
                        return requiredDate;
                  }
                  set
                  {
                        requiredDate = value;
                        OnPropertyChanged ( "RequiredDate" );
                  }
            }

            public DateTime ShippedDate
            {
                  get
                  {
                        return shippedDate;
                  }
                  set
                  {
                        shippedDate = value;
                        OnPropertyChanged ( "ShippedDate" );
                  }
            }

            public int ShipVia
            {
                  get
                  {
                        return shipVia;
                  }
                  set
                  {
                        shipVia = value;
                        OnPropertyChanged ( "ShipVia" );
                  }
            }

            public decimal Freight
            {
                  get
                  {
                        return freight;
                  }
                  set
                  {
                        freight = value;
                        OnPropertyChanged ( "Freight" );
                  }
            }

            public string ShipName
            {
                  get
                  {
                        return shipName;
                  }
                  set
                  {
                        shipName = value;
                        OnPropertyChanged ( "ShipName" );
                  }
            }

            public string ShipAddress
            {
                  get
                  {
                        return shipAddress;
                  }
                  set
                  {
                        shipAddress = value;
                        OnPropertyChanged ( "ShipAddress" );
                  }
            }

            public string ShipCity
            {
                  get
                  {
                        return shipCity;
                  }
                  set
                  {
                        shipCity = value;
                        OnPropertyChanged ( "ShipCity" );
                  }
            }

            public string ShipRegion
            {
                  get
                  {
                        return shipRegion;
                  }
                  set
                  {
                        shipRegion = value;
                        OnPropertyChanged ( "ShipRegion" );
                  }
            }

            public string ShipPostalCode
            {
                  get
                  {
                        return shipPostalCode;
                  }
                  set
                  {
                        shipPostalCode = value;
                        OnPropertyChanged ( "ShipPostalCode" );
                  }
            }

            public string ShipCountry
            {
                  get
                  {
                        return shipCountry;
                  }
                  set
                  {
                        shipCountry = value;
                        OnPropertyChanged ( "ShipCountry" );
                  }
            }
            #endregion declarations
      }
}
